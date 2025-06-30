using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base;
using OdontFlow.Domain.ViewModel.Lab;
using OdontFlow.Domain.ViewModel.StationWork;
using StationWorkModel = OdontFlow.Domain.Entities.StationWork;

namespace OdontFlow.Application.BussinesProcess.StationWork.Query;

public class GetProductivityReportQuery
{
    public string Filtro { get; set; } = "SEMANA_ACTUAL";
}

public class GetProductivityReportQueryHandler(
    IReadOnlyRepository<Guid, StationWorkModel> repository,
    IMapper mapper
) : IQueryHandler<GetStationWorkSummaryQuery, StationWorkSummaryViewModel>
{
    public async Task<StationWorkSummaryViewModel> Handle(GetProductivityReportQuery query)
    {
        var all = await repository.GetAllMatchingAsync(x => x.Active && !x.Deleted, new[] { "WorkStation" });

        var now = DateTime.Now;
        var (desde, hasta) = GetRangoFechas(query.Filtro.ToUpperInvariant());

        var filtrados = all.Where(x =>
            x.StationStartDate.Date >= desde.Date && x.StationStartDate.Date <= hasta.Date).ToList();

        var total = filtrados.Count();
        var enEspera = filtrados.Count(x => x.WorkStatus == WORK_STATUS.ESPERA);
        var enProceso = filtrados.Count(x => x.WorkStatus == WORK_STATUS.PROCESO);
        var terminados = filtrados.Count(x => x.WorkStatus == WORK_STATUS.TERMINADO);
        var bloqueados = filtrados.Count(x => x.WorkStatus == WORK_STATUS.BLOQUEADO);
        var rechazados = filtrados.Count(x => x.WorkStatus == WORK_STATUS.RECHAZADO);

        var trabajosEnCurso = filtrados.Where(x =>
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
            return transcurrido.TotalMinutes > tiempoTotal.TotalMinutes * 0.5 &&
                   transcurrido.TotalMinutes <= tiempoTotal.TotalMinutes * 0.75;
        });

        var conRetraso = trabajosEnCurso.Count(x => now > x.StationEndDate);

        var hoy = DateTime.Today;
        var trabajosTerminadosHoy = filtrados.Count(x => x.WorkStatus == WORK_STATUS.TERMINADO && x.EmployeeEndDate.Date == hoy);
        var trabajosComprometidosHoy = filtrados.Count(x => x.StationEndDate.Date == hoy);

        var productividad = trabajosComprometidosHoy > 0
            ? Math.Round((double)trabajosTerminadosHoy / trabajosComprometidosHoy * 100, 2)
            : 0;

        var porEstacion = filtrados
         .GroupBy(x => x.WorkStation.Id)
         .Select(group => new StationWorkByStationViewModel
         {
             StationId = group.Key,
             StationName = group.First().WorkStation.Name,
             TotalTrabajos = group.Count(),
             TrabajosEnEspera = group.Count(x => x.WorkStatus == WORK_STATUS.ESPERA),
             TrabajosEnProceso = group.Count(x => x.WorkStatus == WORK_STATUS.PROCESO),
             TrabajosTerminados = group.Count(x => x.WorkStatus == WORK_STATUS.TERMINADO),
             TrabajosBloqueados = group.Count(x => x.WorkStatus == WORK_STATUS.BLOQUEADO),
             TrabajosRechazados = group.Count(x => x.WorkStatus == WORK_STATUS.RECHAZADO),
             TrabajosEnTiempo = group.Count(x =>
             {
                 if (x.WorkStatus != WORK_STATUS.ESPERA && x.WorkStatus != WORK_STATUS.PROCESO && x.WorkStatus != WORK_STATUS.RECHAZADO) return false;
                 var tiempoTotal = x.StationEndDate - x.StationStartDate;
                 var transcurrido = now - x.StationStartDate;
                 return transcurrido.TotalMinutes <= tiempoTotal.TotalMinutes * 0.5;
             }),
             TrabajosConAlarma = group.Count(x =>
             {
                 if (x.WorkStatus != WORK_STATUS.ESPERA && x.WorkStatus != WORK_STATUS.PROCESO && x.WorkStatus != WORK_STATUS.RECHAZADO) return false;
                 var tiempoTotal = x.StationEndDate - x.StationStartDate;
                 var transcurrido = now - x.StationStartDate;
                 return transcurrido.TotalMinutes > tiempoTotal.TotalMinutes * 0.5 &&
                        transcurrido.TotalMinutes <= tiempoTotal.TotalMinutes * 0.75;
             }),
             TrabajosConRetraso = group.Count(x =>
             {
                 if (x.WorkStatus != WORK_STATUS.ESPERA && x.WorkStatus != WORK_STATUS.PROCESO && x.WorkStatus != WORK_STATUS.RECHAZADO) return false;
                 return now > x.StationEndDate;
             }),
             TrabajosTerminadosHoy = group.Count(x => x.WorkStatus == WORK_STATUS.TERMINADO && x.EmployeeEndDate.Date == hoy),
             TrabajosComprometidosHoy = group.Count(x => x.StationEndDate.Date == hoy),
             Productividad = group.Count(x => x.StationEndDate.Date == hoy) > 0
                ? Math.Round((double)group.Count(x => x.WorkStatus == WORK_STATUS.TERMINADO && x.EmployeeEndDate.Date == hoy)
                             / group.Count(x => x.StationEndDate.Date == hoy) * 100, 2)
                : 0
         })
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
            Productividad = productividad,
            PorEstacion = porEstacion
        };
    }

    private (DateTime desde, DateTime hasta) GetRangoFechas(string filtro)
    {
        var hoy = DateTime.Today;
        var diaSemana = (int)hoy.DayOfWeek;
        int diasDesdeViernes = (diaSemana + 1) % 7;
        DateTime viernesPasado = hoy.AddDays(-diasDesdeViernes);

        return filtro switch
        {
        "SEMANA_ACTUAL" => (viernesPasado, viernesPasado.AddDays(6)),
        "HACE_2_SEMANAS" => (viernesPasado.AddDays(-14), viernesPasado.AddDays(-8)),
        "MES_ACTUAL" => (new DateTime(hoy.Year, hoy.Month, 1), new DateTime(hoy.Year, hoy.Month, 1).AddMonths(1).AddDays(-1)),
        "MES_ANTERIOR" => {
        var mesAnterior = hoy.AddMonths(-1);
                var inicio = new DateTime(mesAnterior.Year, mesAnterior.Month, 1);
                var fin = inicio.AddMonths(1).AddDays(-1);
                return (inicio, fin);
    },
            _ => throw new ArgumentException("Filtro inválido")
        };
    }
}
