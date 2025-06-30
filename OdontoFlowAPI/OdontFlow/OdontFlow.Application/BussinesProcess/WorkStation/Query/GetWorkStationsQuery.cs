
using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.WorkStation;
using ViewModel = OdontFlow.Domain.ViewModel.WorkStation.WorkStationViewModel;

namespace OdontFlow.Application.BussinesProcess.WorkStation.Query;
public class GetWorkStationsQuery()
{
}
public class GetWorkStationsQueryHandler(
    IReadOnlyRepository<Guid, Model> readOnlyRepository,
    IMapper mapper)
    : IQueryHandler<GetWorkStationsQuery, IEnumerable<ViewModel>>
{
    public async Task<IEnumerable<ViewModel>> Handle(GetWorkStationsQuery query)
    {
        var entities = await readOnlyRepository.GetAllAsync();
        return mapper.Map<IEnumerable<ViewModel>>(entities);
    }
}
