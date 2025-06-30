using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Exceptions;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.Order;
using ViewModel = OdontFlow.Domain.ViewModel.Order.OrderViewModel;
using StationWorkModel = OdontFlow.Domain.Entities.StationWork;
using WorkingHoursModel = OdontFlow.Domain.Entities.WorkingHour;
using HolyDayModel = OdontFlow.Domain.Entities.Holiday;
using WorkPlanModel = OdontFlow.Domain.Entities.WorkPlanProducts;

namespace OdontFlow.Application.BussinesProcess.Order.Command;
public class ConfirmOrderCommand(Guid id) : ICommand<ViewModel>
{
    public Guid Id { get; set; } = id;
}
public class ConfirmOrderCommandHandler(
    IWriteOnlyRepository<Guid, Model> writeRepo,
    IReadOnlyRepository<Guid, Model> readRepo,
    IWriteOnlyRepository<Guid, StationWorkModel> stationWorkRepository,
    IReadOnlyRepository<Guid, WorkingHoursModel> workingHoursRepository,
    IReadOnlyRepository<Guid, HolyDayModel> holyDayRepository,
    IReadOnlyRepository<Guid, WorkPlanModel> workPlanRepository,
    IMapper mapper
) : ICommandHandler<ConfirmOrderCommand, ViewModel>
{
    public async Task<ViewModel> Handle(ConfirmOrderCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        var entity = await readRepo.GetAsync(command.Id, new[] { "Items", "Client", "Client.Employee" })
            ?? throw new NotFoundException("Orden no encontrada.");
   
        await CalculateStationWorks(entity);
        return mapper.Map<ViewModel>(entity);
    }

    private async Task CalculateStationWorks(Model order)
    {
        var now = DateTime.Now;

        var workingHours = await workingHoursRepository.GetAllMatchingAsync(x => x.Active && !x.Deleted);
        var holidays = await holyDayRepository.GetAllMatchingAsync(x => x.Active && !x.Deleted);
        var holidayDates = holidays.Select(h => h.Date.Date).ToHashSet();

        int productCounter = 1;
        var allStationEndDates = new List<DateTime>();

        foreach (var item in order.Items)
        {
            var productBarcode = $"{order.Barcode}{productCounter:D2}";

            var planResult = await workPlanRepository.GetAllMatchingAsync(
                p => p.ProductId == item.ProductId,
                new[] { "Plan", "Plan.Stations", "Plan.Stations.WorkStation" });

            if (planResult == null) continue;

            var plan = planResult.FirstOrDefault();
            if (plan?.Plan?.Stations == null || !plan.Plan.Stations.Any()) continue;

            var currentStart = now;

            foreach (var station in plan.Plan.Stations.OrderBy(s => s.Order))
            {
                var stationDuration = station.WorkStation.Days * 24;
                var stationEnd = await CalculateEndDate(currentStart, stationDuration, workingHours, holidayDates);

                var stationWork = new StationWorkModel
                {
                    Id = Guid.NewGuid(),
                    WorkStationId = station.WorkStationId,
                    WorkStatus = WORK_STATUS.ESPERA,
                    InProgress = false,
                    Step = station.Order,
                    StationStartDate = currentStart,
                    StationEndDate = stationEnd,
                    EmployeeId = null,
                    Barcode = productBarcode,
                    OrderId = order.Id,
                    ProductId = item.ProductId
                };

                allStationEndDates.Add(stationEnd);
                currentStart = stationEnd;
                await stationWorkRepository.AddAsync(stationWork);
            }

            productCounter++;
        }

        // 🔴 Nuevo: actualizar CommitmentDate con la última fecha de entrega
        if (allStationEndDates.Any())
        {
            var copyOrder = await readRepo.GetAsync(order.Id);
            copyOrder.CommitmentDate = allStationEndDates.Max();
            await writeRepo.Modify(copyOrder); // persistir cambio
        }
    }


    private async Task<DateTime> CalculateEndDate(DateTime start, double hoursRequired, IEnumerable<WorkingHoursModel> workingHours, HashSet<DateTime> holidayDates)
    {
        var remaining = TimeSpan.FromHours(hoursRequired);
        var current = start;

        while (remaining > TimeSpan.Zero)
        {
            var dayOfWeek = current.DayOfWeek;
            var workDay = workingHours.FirstOrDefault(h => h.DayOfWeek == dayOfWeek);

            if (workDay == null || holidayDates.Contains(current.Date))
            {
                current = current.Date.AddDays(1);
                continue;
            }

            var workStart = current.Date.Add(workDay.StartTime);
            var workEnd = current.Date.Add(workDay.EndTime);

            if (current < workStart)
                current = workStart;

            if (current >= workEnd)
            {
                current = current.Date.AddDays(1);
                continue;
            }

            var availableToday = workEnd - current;

            if (availableToday >= remaining)
                return current.Add(remaining);

            remaining -= availableToday;
            current = current.Date.AddDays(1);
        }

        return current;
    }

}



