using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base;
using OdontFlow.Domain.ViewModel.StationWork;
using OdontFlow.Domain.ViewModel.Report;
using OdontFlow.Domain.Entities;
using Model = OdontFlow.Domain.Entities.StationWork;
using OdontFlow.Domain.ViewModel.Lab;

namespace OdontFlow.Application.BussinesProcess.StationWork.Query;

public class GetFilteredProductivityReportQuery(DateTime startDate, DateTime endDate, int mode)
{
    public DateTime StartDate { get; set; } = startDate;
    public DateTime EndDate { get; set; } = endDate;
    public int Mode { get; set; } = mode; // 0 = resumen, 1 = detalle por estación, 2 = detalle por empleado
}

public class GetFilteredProductivityReportQueryHandler(
    IReadOnlyRepository<Guid, Model> repository,
    IReadOnlyRepository<Guid, WorkingHour> workingHoursRepo,
    IReadOnlyRepository<Guid, Holiday> holidaysRepo,
    IMapper mapper
) : IQueryHandler<GetFilteredProductivityReportQuery, ProductivityReportViewModel>
{
    public async Task<ProductivityReportViewModel> Handle(GetFilteredProductivityReportQuery query)
    {
        var workingHours = await workingHoursRepo.GetAllMatchingAsync(x => x.Active && !x.Deleted);
        var holidays = await holidaysRepo.GetAllMatchingAsync(x => x.Active && !x.Deleted);
        var holidayDates = holidays.Select(h => h.Date.Date).ToHashSet();

        var all = await repository.GetAllMatchingAsync(
            x => x.Active && !x.Deleted &&
                 x.StationStartDate >= query.StartDate.Date &&
                 x.StationStartDate <= query.EndDate.Date.AddDays(1),
            new[] { "WorkStation", "Employee", "Order", "Product" });

        var now = DateTime.Now;
        var productivos = all.ToList();

        var porEstacion = productivos
            .Where(x => x.WorkStation != null)
            .GroupBy(x => x.WorkStation!.Id)
            .Select(group =>
            {
                var trabajos = group.ToList();
                var tiempoReal = trabajos
                    .Sum(x => CalcularTiempoReal(x.EmployeeStartDate, x.EmployeeEndDate, workingHours, holidayDates, x.WorkStatus));

                var tiempoPlaneado = trabajos
                    .Where(x => x.StationStartDate != default && x.StationEndDate != default)
                    .Sum(x => (x.StationEndDate - x.StationStartDate).TotalMinutes);

                var cumplimiento = (tiempoPlaneado > 0 && tiempoReal > 0)
                    ? Math.Min((tiempoReal / tiempoPlaneado) * 100, 500)
                    : 0;

                var productividad = (tiempoReal > 0 && tiempoPlaneado > 0)
                    ? Math.Min((tiempoPlaneado / tiempoReal) * 100, 500)
                    : 0;

                return new StationWorkByStationViewModel
                {
                    StationId = group.Key,
                    StationName = group.First().WorkStation!.Name,
                    TotalTrabajos = trabajos.Count,
                    TrabajosEnEspera = trabajos.Count(x => x.WorkStatus == WORK_STATUS.ESPERA),
                    TrabajosEnProceso = trabajos.Count(x => x.WorkStatus == WORK_STATUS.PROCESO),
                    TrabajosTerminados = trabajos.Count(x => x.WorkStatus == WORK_STATUS.TERMINADO),
                    TrabajosBloqueados = trabajos.Count(x => x.WorkStatus == WORK_STATUS.BLOQUEADO),
                    TrabajosRechazados = trabajos.Count(x => x.WorkStatus == WORK_STATUS.RECHAZADO),
                    Cumplimiento = $"{cumplimiento:F0}%",
                    Productividad = $"{productividad:F0}%",
                };
            })
            .OrderBy(x => x.StationName)
            .ToList();

        var porEmpleado = productivos
            .Where(x => x.Employee != null)
            .GroupBy(x => x.Employee!.Id)
            .Select(group =>
            {
                var trabajos = group.ToList();
                var tiempoReal = trabajos
                    .Sum(x => CalcularTiempoReal(x.EmployeeStartDate, x.EmployeeEndDate, workingHours, holidayDates, x.WorkStatus));

                var tiempoPlaneado = trabajos
                    .Where(x => x.StationStartDate != default && x.StationEndDate != default)
                    .Sum(x => (x.StationEndDate - x.StationStartDate).TotalMinutes);

                var cumplimiento = (tiempoPlaneado > 0 && tiempoReal > 0)
                    ? Math.Min((tiempoReal / tiempoPlaneado) * 100, 500)
                    : 0;

                var productividad = (tiempoReal > 0 && tiempoPlaneado > 0)
                    ? Math.Min((tiempoPlaneado / tiempoReal) * 100, 500)
                    : 0;

                return new StationWorkByEmployeeViewModel
                {
                    EmployeeId = group.Key,
                    EmployeeName = group.First().Employee!.Name,
                    TotalTrabajos = trabajos.Count,
                    TrabajosEnEspera = trabajos.Count(x => x.WorkStatus == WORK_STATUS.ESPERA),
                    TrabajosEnProceso = trabajos.Count(x => x.WorkStatus == WORK_STATUS.PROCESO),
                    TrabajosTerminados = trabajos.Count(x => x.WorkStatus == WORK_STATUS.TERMINADO),
                    TrabajosBloqueados = trabajos.Count(x => x.WorkStatus == WORK_STATUS.BLOQUEADO),
                    TrabajosRechazados = trabajos.Count(x => x.WorkStatus == WORK_STATUS.RECHAZADO),
                    Cumplimiento = $"{cumplimiento:F0}%",
                    Productividad = $"{productividad:F0}%",
                };
            })
            .OrderBy(x => x.EmployeeName)
            .ToList();

        List<StationWorkDetailViewModel>? detalle = null;
        if (query.Mode == 1 || query.Mode == 2)
        {
            detalle = productivos
                .Where(x => x.WorkStatus == WORK_STATUS.TERMINADO)
                .Select(sw =>
                {
                    var tiempoPlaneado = (sw.StationEndDate - sw.StationStartDate).TotalMinutes;
                    var tiempoReal = CalcularTiempoReal(sw.EmployeeStartDate, sw.EmployeeEndDate, workingHours, holidayDates, sw.WorkStatus);

                    return new StationWorkDetailViewModel
                    {
                        OrderId = sw.OrderId ?? Guid.Empty,
                        OrderNumber = sw.Barcode,
                        WorkStationName = sw.WorkStation?.Name ?? "",
                        ClientName = sw.Order?.Client?.Name ?? "",
                        ProductName = sw.Order?.Items?.FirstOrDefault(i => i.ProductId == sw.ProductId)?.Product?.Name ?? "",
                        StationStartDate = sw.StationStartDate,
                        StationEndDate = sw.StationEndDate,
                        EmployeeStartDate = sw.EmployeeStartDate,
                        EmployeeEndDate = sw.EmployeeEndDate,
                        RealTime = $"{tiempoReal:F0} min",
                        EstimatedTime = $"{tiempoPlaneado:F0} min",
                        ProductivityPercent = tiempoPlaneado > 0 ? $"{(tiempoReal / tiempoPlaneado * 100):F0}%" : "N/A",
                        CurrentEmployee = sw.Employee?.Name
                    };
                })
                .ToList();
        }

        return new ProductivityReportViewModel
        {
            TotalTrabajos = productivos.Count,
            TrabajosEnEspera = productivos.Count(x => x.WorkStatus == WORK_STATUS.ESPERA),
            TrabajosEnProceso = productivos.Count(x => x.WorkStatus == WORK_STATUS.PROCESO),
            TrabajosTerminados = productivos.Count(x => x.WorkStatus == WORK_STATUS.TERMINADO),
            TrabajosBloqueados = productivos.Count(x => x.WorkStatus == WORK_STATUS.BLOQUEADO),
            TrabajosRechazados = productivos.Count(x => x.WorkStatus == WORK_STATUS.RECHAZADO),
            TrabajosEnTiempo = 0,
            TrabajosConAlarma = 0,
            TrabajosConRetraso = 0,
            TrabajosTerminadosHoy = productivos.Count(x => x.WorkStatus == WORK_STATUS.TERMINADO && x.EmployeeEndDate.Date == DateTime.Today),
            TrabajosComprometidosHoy = productivos.Count(x => x.StationEndDate.Date == DateTime.Today),
            PorEstacion = porEstacion,
            PorEmpleado = porEmpleado,
            DetallePorEstacion = query.Mode == 1 ? detalle : null,
            DetallePorEmpleado = query.Mode == 2 ? detalle : null
        };
    }

    private static double CalcularTiempoReal(
        DateTime inicio,
        DateTime fin,
        IEnumerable<WorkingHour> workingHours,
        HashSet<DateTime> holidays,
        WORK_STATUS status)
    {
        if (inicio == default || fin == default || inicio >= fin)
            return 0;

        // Si está TERMINADO, se permite contar fuera de horario
        if (status == WORK_STATUS.TERMINADO)
            return (fin - inicio).TotalMinutes;

        double totalMinutes = 0;
        var current = inicio;

        while (current < fin)
        {
            var dayOfWeek = current.DayOfWeek;
            var workDay = workingHours.FirstOrDefault(h => h.DayOfWeek == dayOfWeek);

            if (workDay == null || holidays.Contains(current.Date))
            {
                current = current.Date.AddDays(1);
                continue;
            }

            var workStart = current.Date.Add(workDay.StartTime);
            var workEnd = current.Date.Add(workDay.EndTime);

            var actualStart = current > workStart ? current : workStart;
            var actualEnd = fin < workEnd ? fin : workEnd;

            if (actualStart < actualEnd)
            {
                totalMinutes += (actualEnd - actualStart).TotalMinutes;
            }

            current = current.Date.AddDays(1);
        }

        return totalMinutes;
    }
}
