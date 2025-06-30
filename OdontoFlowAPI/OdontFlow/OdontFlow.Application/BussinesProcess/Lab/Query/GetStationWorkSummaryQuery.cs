using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base;
using OdontFlow.Domain.ViewModel.StationWork;
using OdontFlow.Domain.ViewModel.Lab;
using StationWorkModel = OdontFlow.Domain.Entities.StationWork;

namespace OdontFlow.Application.BussinesProcess.StationWork.Query;

public class GetStationWorkSummaryQuery() { }

public class GetStationWorkSummaryQueryHandler(
    IReadOnlyRepository<Guid, StationWorkModel> repository,
    IMapper mapper
) : IQueryHandler<GetStationWorkSummaryQuery, StationWorkSummaryViewModel>
{
    public async Task<StationWorkSummaryViewModel> Handle(GetStationWorkSummaryQuery query)
    {
        var all = await repository.GetAllMatchingAsync(x => x.Active && !x.Deleted, new[] { "WorkStation", "Employee" });

        var now = DateTime.Now;
        var hoy = DateTime.Today;

        var productivos = all.ToList();

        var total = productivos.Count();
        var enEspera = productivos.Count(x => x.WorkStatus == WORK_STATUS.ESPERA);
        var enProceso = productivos.Count(x => x.WorkStatus == WORK_STATUS.PROCESO);
        var terminados = productivos.Count(x => x.WorkStatus == WORK_STATUS.TERMINADO);
        var bloqueados = all.Count(x => x.WorkStatus == WORK_STATUS.BLOQUEADO);
        var rechazados = productivos.Count(x => x.WorkStatus == WORK_STATUS.RECHAZADO);

        var trabajosEnCurso = productivos.Where(x =>
            x.WorkStatus == WORK_STATUS.ESPERA || x.WorkStatus == WORK_STATUS.PROCESO);

        var enTiempo = trabajosEnCurso.Count(x =>
        {
            var tiempoTotal = x.StationEndDate - x.StationStartDate;
            var transcurrido = now - x.StationStartDate;
            return transcurrido.TotalMinutes <= tiempoTotal.TotalMinutes * 0.5;
        });

        var conAlarma = trabajosEnCurso.Count(x =>
        {
            var tiempoTotal = x.StationEndDate - x.StationStartDate;
            var transcurrido = now - x.StationStartDate;
            return transcurrido.TotalMinutes > tiempoTotal.TotalMinutes * 0.5
                && transcurrido.TotalMinutes <= tiempoTotal.TotalMinutes * 0.75;
        });

        var conRetraso = trabajosEnCurso.Count(x => now > x.StationEndDate);

        var trabajosTerminadosHoy = productivos.Count(x => x.WorkStatus == WORK_STATUS.TERMINADO && x.EmployeeEndDate.Date == hoy);
        var trabajosComprometidosHoy = productivos.Count(x => x.StationEndDate.Date == hoy);

        var porEstacion = productivos
            .GroupBy(x => x.WorkStation.Id)
            .Select(group =>
            {
                var name = group.First().WorkStation.Name.ToLower();
                int orden = name switch
                {
                    "yesos" => 1,
                    "diseño y fresado" => 2,
                    "tallado y ajuste" => 3,
                    "ceramica y terminado" => 4,
                    _ => 99
                };

                var trabajosEstacion = group.ToList();
                var trabajosEnCursoEstacion = trabajosEstacion
                    .Where(x => x.WorkStatus == WORK_STATUS.ESPERA || x.WorkStatus == WORK_STATUS.PROCESO || x.WorkStatus == WORK_STATUS.RECHAZADO);

                return new StationWorkByStationViewModel
                {
                    StationId = group.Key,
                    StationName = group.First().WorkStation.Name,
                    TotalTrabajos = trabajosEstacion.Count,
                    TrabajosEnEspera = trabajosEstacion.Count(x => x.WorkStatus == WORK_STATUS.ESPERA),
                    TrabajosEnProceso = trabajosEstacion.Count(x => x.WorkStatus == WORK_STATUS.PROCESO),
                    TrabajosTerminados = trabajosEstacion.Count(x => x.WorkStatus == WORK_STATUS.TERMINADO),
                    TrabajosBloqueados = all.Count(x => x.WorkStation.Id == group.Key && x.WorkStatus == WORK_STATUS.BLOQUEADO),
                    TrabajosRechazados = trabajosEstacion.Count(x => x.WorkStatus == WORK_STATUS.RECHAZADO),
                    TrabajosEnTiempo = trabajosEnCursoEstacion.Count(x =>
                    {
                        var tiempoTotal = x.StationEndDate - x.StationStartDate;
                        var transcurrido = now - x.StationStartDate;
                        return transcurrido.TotalMinutes <= tiempoTotal.TotalMinutes * 0.5;
                    }),
                    TrabajosConAlarma = trabajosEnCursoEstacion.Count(x =>
                    {
                        var tiempoTotal = x.StationEndDate - x.StationStartDate;
                        var transcurrido = now - x.StationStartDate;
                        return transcurrido.TotalMinutes > tiempoTotal.TotalMinutes * 0.5
                            && transcurrido.TotalMinutes <= tiempoTotal.TotalMinutes * 0.75;
                    }),
                    TrabajosConRetraso = trabajosEnCursoEstacion.Count(x => now > x.StationEndDate),
                    TrabajosTerminadosHoy = trabajosEstacion.Count(x => x.WorkStatus == WORK_STATUS.TERMINADO && x.EmployeeEndDate.Date == hoy),
                    TrabajosComprometidosHoy = trabajosEstacion.Count(x => x.StationEndDate.Date == hoy),
                    Orden = orden
                };
            })
            .ToList();

        var porEmpleado = productivos
            .Where(x => x.Employee != null)
            .GroupBy(x => x.Employee.Id)
            .Select(group =>
            {
                var trabajosEmpleado = group.ToList();
                var trabajosEnCursoEmpleado = trabajosEmpleado
                    .Where(x => x.WorkStatus == WORK_STATUS.ESPERA || x.WorkStatus == WORK_STATUS.PROCESO || x.WorkStatus == WORK_STATUS.RECHAZADO);

                return new StationWorkByEmployeeViewModel
                {
                    EmployeeId = group.Key,
                    EmployeeName = group.First().Employee.Name,
                    TotalTrabajos = trabajosEmpleado.Count,
                    TrabajosEnEspera = trabajosEmpleado.Count(x => x.WorkStatus == WORK_STATUS.ESPERA),
                    TrabajosEnProceso = trabajosEmpleado.Count(x => x.WorkStatus == WORK_STATUS.PROCESO),
                    TrabajosTerminados = trabajosEmpleado.Count(x => x.WorkStatus == WORK_STATUS.TERMINADO),
                    TrabajosBloqueados = all.Count(x => x.Employee != null && x.Employee.Id == group.Key && x.WorkStatus == WORK_STATUS.BLOQUEADO),
                    TrabajosRechazados = trabajosEmpleado.Count(x => x.WorkStatus == WORK_STATUS.RECHAZADO),

                    TrabajosEnTiempo = trabajosEnCursoEmpleado.Count(x =>
                    {
                        var tiempoTotal = x.StationEndDate - x.StationStartDate;
                        var transcurrido = now - x.StationStartDate;
                        return transcurrido.TotalMinutes <= tiempoTotal.TotalMinutes * 0.5;
                    }),
                    TrabajosConAlarma = trabajosEnCursoEmpleado.Count(x =>
                    {
                        var tiempoTotal = x.StationEndDate - x.StationStartDate;
                        var transcurrido = now - x.StationStartDate;
                        return transcurrido.TotalMinutes > tiempoTotal.TotalMinutes * 0.5
                            && transcurrido.TotalMinutes <= tiempoTotal.TotalMinutes * 0.75;
                    }),
                    TrabajosConRetraso = trabajosEnCursoEmpleado.Count(x => now > x.StationEndDate),

                    TrabajosTerminadosHoy = trabajosEmpleado.Count(x => x.WorkStatus == WORK_STATUS.TERMINADO && x.EmployeeEndDate.Date == hoy),
                    TrabajosComprometidosHoy = trabajosEmpleado.Count(x => x.StationEndDate.Date == hoy)
                };
            })
            .OrderBy(x => x.EmployeeName)
            .ToList();

        return new StationWorkSummaryViewModel
        {
            TotalTrabajos = total,
            TrabajosEnEspera = enEspera,
            TrabajosEnProceso = enProceso,
            TrabajosTerminados = terminados,
            TrabajosBloqueados = bloqueados,
            TrabajosRechazados = rechazados,
            TrabajosEnTiempo = enTiempo,
            TrabajosConAlarma = conAlarma,
            TrabajosConRetraso = conRetraso,
            TrabajosTerminadosHoy = trabajosTerminadosHoy,
            TrabajosComprometidosHoy = trabajosComprometidosHoy,
            PorEstacion = porEstacion.OrderBy(o => o.Orden).ToList(),
            PorEmpleado = porEmpleado
        };
    }
}
