using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.OrderPayment;
using ViewModel = OdontFlow.Domain.ViewModel.OrderPayment.OrderPaymentViewModel;
using UpdateDTO = OdontFlow.Domain.DTOs.OrderPayment.UpdateOrderPaymentDTO;
using OrderModel = OdontFlow.Domain.Entities.Order;
using OdontFlow.CrossCutting.Exceptions;
using Mapster;

namespace OdontFlow.Application.BussinesProcess.OrderPayment.Command;

public class UpdateOrderPaymentCommand(UpdateDTO input) : ICommand<ViewModel>
{
    public UpdateDTO Input { get; set; } = input;
}
public class UpdateOrderPaymentCommandHandler(
    IReadOnlyRepository<Guid, OrderModel> readOrderRepository,
    IWriteOnlyRepository<Guid, OrderModel> writeOrderRepository,
    IWriteOnlyRepository<Guid, Model> writeRepo,
    IReadOnlyRepository<Guid, Model> readRepo,
    IMapper mapper
) : ICommandHandler<UpdateOrderPaymentCommand, ViewModel>
{
    public async Task<ViewModel> Handle(UpdateOrderPaymentCommand command)
    {
        var entity = await readRepo.GetAsync(command.Input.Id)
            ?? throw new NotFoundException("Pago no encontrado.");

        // Actualiza los campos del pago
        command.Input.Adapt(entity);
        await writeRepo.Modify(entity);

        // Obtener la orden asociada
        var order = await readOrderRepository.GetAsync(entity.OrderId)
            ?? throw new NotFoundException("Orden asociada no encontrada.");

        // --- Recalcular pagos y balance ---
        var allPayments = await readRepo.GetAllMatchingAsync(p => p.OrderId == order.Id && !p.Deleted);
        var totalPaid = allPayments.Sum(p => p.Amount);

        order.Payment = totalPaid;
        order.Balance = order.Total - order.Payment;

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
