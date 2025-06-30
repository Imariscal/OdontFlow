
using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.Client;
using ViewModel = OdontFlow.Domain.ViewModel.Client.ClientViewModel;

namespace OdontFlow.Application.BussinesProcess.PriceList.Query;
public class GetClientsQuery()
{
}
public class GetClientsQueryHandler(
    IReadOnlyRepository<Guid, Model> readOnlyRepository,
    IMapper mapper)
    : IQueryHandler<GetClientsQuery, IEnumerable<ViewModel>>
{
    public async Task<IEnumerable<ViewModel>> Handle(GetClientsQuery query)
    {
        var entities = await readOnlyRepository.GetAllAsync(new[] { "ClientInvoice", "Employee", "PriceList" });
        return mapper.Map<IEnumerable<ViewModel>>(entities);
    }
}
