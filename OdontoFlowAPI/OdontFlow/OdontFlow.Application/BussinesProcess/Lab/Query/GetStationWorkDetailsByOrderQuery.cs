using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base;
using ViewModel = OdontFlow.Domain.ViewModel.StationWork.StationWorkDetailViewModel;
using Model = OdontFlow.Domain.Entities.StationWork;

namespace OdontFlow.Application.BussinesProcess.Lab.Query
{
    public class GetStationWorkDetailsByOrderQuery(Guid orderId)
    {
        public Guid OrderId { get; set; } = orderId;
    }

    public class GetStationWorkDetailsByOrderQueryHanlder(
       IReadOnlyRepository<Guid, Model> stationWorkRepo,
       IMapper mapper
   ) : IQueryHandler<GetStationWorkDetailsByOrderQuery, IEnumerable<ViewModel>>
    {
        public async Task<IEnumerable<ViewModel>> Handle(GetStationWorkDetailsByOrderQuery query)
        {     
            var entities = await stationWorkRepo.GetAllMatchingAsync(
                sw =>  sw.OrderId == query.OrderId,
                new[] { "WorkStation", "Employee", "Order", "Order.Client", "Order.Items", "Order.Items.Product" });



            return mapper.Map<IEnumerable<ViewModel>>(entities);
        }

    }
}