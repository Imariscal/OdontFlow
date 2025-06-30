using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Exceptions;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.OrderPayment;
using ViewModel = OdontFlow.Domain.ViewModel.OrderPayment.OrderPaymentViewModel;
using OrderModel = OdontFlow.Domain.Entities.Order;
using OdontFlow.Domain.Entities;

namespace OdontFlow.Application.BussinesProcess.OrderPayment.Command;

public class DeleteOrderPaymentCommand(Guid id) : ICommand<ViewModel>
{
    public Guid Id { get; set; } = id;
}

public class DeleteOrderPaymentCommandHandler(
    IReadOnlyRepository<Guid, OrderModel> readOrderRepository,
    IWriteOnlyRepository<Guid, OrderModel> writeOrderRepository,
    IWriteOnlyRepository<Guid, Model> repository,
    IReadOnlyRepository<Guid, Model> readRepository,
    IMapper mapper
) : ICommandHandler<DeleteOrderPaymentCommand, ViewModel>
{
    public async Task<ViewModel> Handle(DeleteOrderPaymentCommand command)
    {
        var entity = await readRepository.GetAsync(command.Id)
            ?? throw new NotFoundException("Orden de pago no encontrada.");

        await repository.Remove(entity, true);

        // Obtener la orden asociada
        var order = await readOrderRepository.GetAsync(entity.OrderId)
            ?? throw new NotFoundException("Orden asociada no encontrada.");

        // Recalcular pagos actuales
        var remainingPayments = await readRepository.GetAllMatchingAsync(p => p.OrderId == order.Id && !p.Deleted);
        var totalPaid = remainingPayments.Sum(p => p.Amount);

        // Actualizar el pago acumulado
        order.Payment = totalPaid;

        // Balance = Total - Payment
        order.Balance = order.Total - order.Payment;

        // Asegurar que no sea negativo
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

        return mapper.Map<ViewModel>(entity);
    }
}


