
using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.Client;
using ViewModel = OdontFlow.Domain.ViewModel.Client.ClientViewModel;

namespace OdontFlow.Application.BussinesProcess.PriceList.Query;
public class GetClientsActiveQuery()
{
}
public class GetClientsActiveQueryHandler(
    IReadOnlyRepository<Guid, Model> readOnlyRepository,
    IMapper mapper)
    : IQueryHandler<GetClientsActiveQuery, IEnumerable<ViewModel>>
{
    public async Task<IEnumerable<ViewModel>> Handle(GetClientsActiveQuery query)
    {
        var entities = await readOnlyRepository.GetAllMatchingAsync(c => c.Active && !c.Deleted, new[] { "ClientInvoice", "Employee", "PriceList" });
        return mapper.Map<IEnumerable<ViewModel>>(entities);
    }
}
