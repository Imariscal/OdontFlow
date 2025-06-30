using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Exceptions;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.Order;
using ItemModel = OdontFlow.Domain.Entities.OrderItem;
using ViewModel = OdontFlow.Domain.ViewModel.Order.OrderViewModel;
using UpdateDTO = OdontFlow.Domain.DTOs.Order.UpdateOrderDTO;
using ClientModel = OdontFlow.Domain.Entities.Client;
using StationWorkModel = OdontFlow.Domain.Entities.StationWork;
using WorkPlanModel = OdontFlow.Domain.Entities.WorkPlanProducts;
using WorkingHoursModel = OdontFlow.Domain.Entities.WorkingHour;
using HolyDayModel = OdontFlow.Domain.Entities.Holiday;
using ModelEmployee = OdontFlow.Domain.Entities.Employee;
using Mapster;
using OdontFlow.Domain.Entities;

namespace OdontFlow.Application.BussinesProcess.Order.Command;
public class UpdateOrderCommand(UpdateDTO input) : ICommand<ViewModel>
{
    public UpdateDTO Input { get; set; } = input;
}

public class UpdateOrderCommandHandler(
    IWriteOnlyRepository<Guid, Model> writeRepo,
    IWriteOnlyRepository<Guid, ItemModel> writeItemsRepo,
    IReadOnlyRepository<Guid, Model> readRepo,
    IReadOnlyRepository<Guid, ClientModel> clientRepository,
    IReadOnlyRepository<Guid, StationWorkModel> stationWorkRepo,
    IWriteOnlyRepository<Guid, StationWorkModel> stationWorkWriteRepo,
    IReadOnlyRepository<Guid, WorkPlanModel> workPlanRepository,
    IReadOnlyRepository<Guid, WorkingHoursModel> workingHoursRepository,
    IReadOnlyRepository<Guid, HolyDayModel> holyDayRepository,
    IReadOnlyRepository<Guid, ModelEmployee> employeeReadOnlyRepository,
    IMapper mapper
) : ICommandHandler<UpdateOrderCommand, ViewModel>
{
    public async Task<ViewModel> Handle(UpdateOrderCommand command)
    {
        ArgumentNullException.ThrowIfNull(command.Input);
        var entity = await readRepo.GetAsync(command.Input.Id, new[] { "Items", "Client", "Payments", "Client.Employee" })
            ?? throw new NotFoundException("Orden no encontrada.");

        var isConfirmed = entity.OrderStatusId == ORDER_STATUS.CONFIRMADA;

        // 👉 Guardar el valor original del CommitmentDate
        var originalCommitmentDate = entity.CommitmentDate;
        // Actualizamos datos principales de la orden
        command.Input.Adapt(entity);

        var inputItems = command.Input.Items;
        var currentItems = entity.Items.ToList();
        var itemsToAdd = new List<ItemModel>();

        // 👉 Si la orden aplica factura, recalculamos UnitTax y TotalCost
        foreach (var input in inputItems)
        {
            var unitCost = input.UnitCost;
            var unitTax = 0M;

            if (command.Input.ApplyInvoice)
            {
                unitTax = Math.Round(unitCost * 0.16M, 2);
            }

            var totalUnit = Math.Round(unitCost + unitTax, 2);
            var totalCost = Math.Round(totalUnit * input.Quantity, 2);

            var exists = currentItems.FirstOrDefault(i => i.ProductId == input.ProductId);

            if (exists != null)
            {
                exists.Quantity = input.Quantity;
                exists.UnitCost = unitCost;
                exists.UnitTax = unitTax;
                exists.TotalCost = totalCost;
                exists.Teeth = input.Teeth;
            }
            else
            {
                itemsToAdd.Add(new ItemModel
                {
                    Id = Guid.NewGuid(),
                    OrderId = entity.Id,
                    ProductId = input.ProductId,
                    Quantity = input.Quantity,
                    UnitCost = unitCost,
                    UnitTax = unitTax,
                    TotalCost = totalCost,
                    Teeth = input.Teeth
                });
            }
        }

        // Guardamos los nuevos Items
        if (itemsToAdd.Any())
        {
            await writeItemsRepo.AddRangeAsync(itemsToAdd);

            var lastStationDate = await stationWorkRepo.GetAllMatchingAsync(sw => sw.OrderId == entity.Id);
            var lastEnd = lastStationDate.Any() ? lastStationDate.Max(x => x.StationEndDate) : DateTime.Now;

            var workingHours = await workingHoursRepository.GetAllMatchingAsync(x => x.Active && !x.Deleted);
            var holidays = await holyDayRepository.GetAllMatchingAsync(x => x.Active && !x.Deleted);
            var holidayDates = holidays.Select(h => h.Date.Date).ToHashSet();

            var now = lastEnd;
            int productCounter = entity.Items.Count + 1;

            foreach (var newItem in itemsToAdd)
            {
                var plan = await workPlanRepository.GetAllMatchingAsync(
                    p => p.ProductId == newItem.ProductId,
                    new[] { "Plan", "Plan.Stations", "Plan.Stations.WorkStation" });

                var stationPlan = plan.FirstOrDefault()?.Plan?.Stations?.OrderBy(s => s.Order);

                if (stationPlan != null)
                {
                    var currentStart = now;
                    var productBarcode = $"{entity.Barcode}{productCounter:D2}";

                    foreach (var station in stationPlan)
                    {
                        var duration = station.WorkStation.Days;
                        var stationEnd = await CalculateEndDate(currentStart, duration, workingHours, holidayDates);

                        var stationWork = new StationWorkModel
                        {
                            Id = Guid.NewGuid(),
                            OrderId = entity.Id,
                            WorkStationId = station.WorkStationId,
                            Step = station.Order,
                            StationStartDate = currentStart,
                            StationEndDate = stationEnd,
                            WorkStatus = WORK_STATUS.ESPERA,
                            InProgress = false,
                            Barcode = productBarcode,
                            ProductId = newItem.ProductId
                        };

                        await stationWorkWriteRepo.AddAsync(stationWork);
                        currentStart = stationEnd;
                    }

                    productCounter++;
                    now = now.AddMinutes(1);
                }
            }
        }

        // 👉 Recalcular Totales ahora con los Items correctos
        entity.Subtotal = entity.Items.Sum(i => i.UnitCost * i.Quantity);

        if (command.Input.ApplyInvoice)
        {
            entity.Tax = Math.Round(entity.Subtotal * 0.16M, 2);
        }
        else
        {
            entity.Tax = 0M;
        }

        entity.Total = entity.Subtotal + entity.Tax;
        entity.Payment = entity.Payments.Sum(p => p.Amount);
        entity.Balance = entity.Total - entity.Payment;

        if (entity.Balance <= 0)
        {
            entity.Balance = 0;
            entity.PaymentComplete = true;
            entity.PaymentDate = entity.PaymentDate ?? DateTime.Now;
        }
        else
        {
            entity.PaymentComplete = false;
        }

        // 🔹 Actualizar CommitmentDate si hay nuevas estaciones
        if (itemsToAdd.Any())
        {
            var updatedStations = await stationWorkRepo.GetAllMatchingAsync(sw => sw.OrderId == entity.Id);
            if (updatedStations.Any())
            {
                entity.CommitmentDate = updatedStations.Max(x => x.StationEndDate);
            }
        }
        else
        {
            // 🔹 Mantener la fecha original si no se agregaron nuevas estaciones
            entity.CommitmentDate = originalCommitmentDate;
        }

        await writeRepo.Modify(entity);

        entity.Client = await clientRepository.GetAsync(entity.ClientId);
        entity.Client.Employee = await employeeReadOnlyRepository.GetAsync(entity.Client.EmployeeId);
        return mapper.Map<ViewModel>(entity);
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
