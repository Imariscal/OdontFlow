using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.Order;
using ItemModel = OdontFlow.Domain.Entities.OrderItem;
using ClientModel = OdontFlow.Domain.Entities.Client;
using ViewModel = OdontFlow.Domain.ViewModel.Order.OrderViewModel;
using CreateDTO = OdontFlow.Domain.DTOs.Order.CreateOrderDTO;
using OdontFlow.Application.Services.Contracts;
using ModelEmployee = OdontFlow.Domain.Entities.Employee;

namespace OdontFlow.Application.BussinesProcess.Order.Command;

public class CreateOrderCommand(CreateDTO input) : ICommand<ViewModel>
{
    public CreateDTO Input { get; set; } = input;
}

public class CreateOrderCommandHandler(
    IWriteOnlyRepository<Guid, Model> writeRepository,
    IReadOnlyRepository<Guid, ClientModel> clientRepository,
    IOrderSequenceService orderSequenceService,
    IReadOnlyRepository<Guid, ModelEmployee> employeeReadOnlyRepository,
    IMapper mapper) : ICommandHandler<CreateOrderCommand, ViewModel>
{
    public async Task<ViewModel> Handle(CreateOrderCommand command)
    {
        ArgumentNullException.ThrowIfNull(command.Input);

        var order = mapper.Map<Model>(command.Input);
        var client = await clientRepository.GetAsync(order.ClientId);
        order.Barcode = await orderSequenceService.GenerateBarcodeAsync();

        // 👉 Generamos los Items correctamente
        order.Items = command.Input.Items.Select(i =>
        {
            var unitCost = i.UnitCost;
            var unitTax = 0M;

            if (command.Input.ApplyInvoice)
            {
                unitTax = Math.Round(unitCost * 0.16M, 2);
            }

            var totalUnit = Math.Round(unitCost + unitTax, 2);
            var totalCost = Math.Round(totalUnit * i.Quantity, 2);

            return new ItemModel
            {
                Id = Guid.NewGuid(),
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitCost = unitCost,  
                UnitTax = unitTax,  
                TotalCost = totalCost,
                Teeth = i.Teeth
            };
        }).ToList();

        order.OrderStatusId = ORDER_STATUS.CONFIRMADA;
        order.ConfirmDate = DateTime.Now;
         
        order.Subtotal = order.Items.Sum(i => i.UnitCost * i.Quantity);

        if (command.Input.ApplyInvoice)
        {
            order.Tax = Math.Round(order.Subtotal * 0.16M, 2);
        }
        else
        {
            order.Tax = 0M;
        }

        order.Total = order.Subtotal + order.Tax;

        order.Payment = 0M;
        order.Balance = order.Total;
        order.PaymentComplete = false;
        order.PaymentDate = null;

        order.CommissionPercentage =  client!.CommissionPercentage;
        await writeRepository.AddAsync(order);

        order.Client = client;
        order.Client.Employee = await employeeReadOnlyRepository.GetAsync(order.Client.EmployeeId);
  
        return mapper.Map<ViewModel>(order);
    }

}


