using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.OrderPayment;
using ViewModel = OdontFlow.Domain.ViewModel.OrderPayment.OrderPaymentViewModel;

namespace OdontFlow.Application.BussinesProcess.OrderPayment.Query;

public class GetOrderPaymentsQuery (Guid id) {
    public Guid Id { get; set; } = id;
}

public class GetOrderPaymentsQueryHandler(
    IReadOnlyRepository<Guid, Model> readOnlyRepository,
    IMapper mapper
) : IQueryHandler<GetOrderPaymentsQuery, IEnumerable<ViewModel>>
{
    public async Task<IEnumerable<ViewModel>> Handle(GetOrderPaymentsQuery query)
    {
        var entities = await readOnlyRepository.GetAllMatchingAsync(o => o.OrderId ==query.Id);
        return mapper.Map<IEnumerable<ViewModel>>(entities);
    }
}
