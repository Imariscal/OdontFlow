using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.OrderPayment;
using OrderModel = OdontFlow.Domain.Entities.Order;
using ViewModel = OdontFlow.Domain.ViewModel.OrderPayment.OrderPaymentViewModel;
using CreateDTO = OdontFlow.Domain.DTOs.OrderPayment.CreateOrderPaymentDTO;
using OdontFlow.CrossCutting.Exceptions;

namespace OdontFlow.Application.BussinesProcess.OrderPayment.Command;

public class CreateOrderPaymentCommand(CreateDTO input) : ICommand<ViewModel>
{
    public CreateDTO Input { get; set; } = input;
}

public class CreateOrderPaymentCommandHandler(
    IReadOnlyRepository<Guid, OrderModel> readOrderRepository,
    IWriteOnlyRepository<Guid, OrderModel> writeOrderRepository,
    IWriteOnlyRepository<Guid, Model> writeRepository,
    IMapper mapper
) : ICommandHandler<CreateOrderPaymentCommand, ViewModel>
{
    public async Task<ViewModel> Handle(CreateOrderPaymentCommand command)
    {
        var orderPayment = mapper.Map<Model>(command.Input);

        var order = await readOrderRepository.GetAsync(orderPayment.OrderId)
            ?? throw new NotFoundException("Orden no encontrada.");

        await writeRepository.AddAsync(orderPayment);

        // --- Recalcular el total de pagos (después de agregar el nuevo) ---
        var payment = order.Payments.Sum(p => p.Amount) + orderPayment.Amount;

        // Balance = Total - Payment
        var balance = order.Total - payment;

        // --- Asignar nuevos valores ---
        order.Payment = payment;
        order.Balance = balance;

        if (order.Balance <= 0)
        {
            order.Balance = 0;
            order.PaymentComplete = true;
            order.PaymentDate = DateTime.Now;
        }
        else
        {
            order.PaymentComplete = false; 
        }

        await writeOrderRepository.Modify(order);

        return mapper.Map<ViewModel>(orderPayment);
    }
}

