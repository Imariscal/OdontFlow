
using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.WorkStation;
using ViewModel = OdontFlow.Domain.ViewModel.WorkStation.WorkStationViewModel;

namespace OdontFlow.Application.BussinesProcess.WorkStation.Query;
public class GetWorkStationsActiveQuery()
{
}
public class GetWorkStationsActiveQueryHandler(
    IReadOnlyRepository<Guid, Model> readOnlyRepository,
    IMapper mapper)
    : IQueryHandler<GetWorkStationsActiveQuery, IEnumerable<ViewModel>>
{
    public async Task<IEnumerable<ViewModel>> Handle(GetWorkStationsActiveQuery query)
    {
        var entities = await readOnlyRepository.GetAllAsync(true);
        return mapper.Map<IEnumerable<ViewModel>>(entities);
    }
}
