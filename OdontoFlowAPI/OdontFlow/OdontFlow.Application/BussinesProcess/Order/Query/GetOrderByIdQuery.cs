using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Exceptions;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.Order;
using ViewModel = OdontFlow.Domain.ViewModel.Order.OrderViewModel;

namespace OdontFlow.Application.BussinesProcess.Order.Query;

public class GetOrderByIdQuery(Guid id)
{
    public Guid Id { get; set; } = id;
}

public class GetOrderByIdQueryHandler(
    IReadOnlyRepository<Guid, Model> readOnlyRepository,
    IMapper mapper
) : IQueryHandler<GetOrderByIdQuery, ViewModel>
{
    public async Task<ViewModel> Handle(GetOrderByIdQuery query)
    {
        var entity = await readOnlyRepository.GetAsync(query.Id, 
            new[] { "Client", "Items", "Items.Product", "Payments", "StationWorks", "StationWorks.WorkStation", "StationWorks.Employee", "Client.Employee" })
            ?? throw new NotFoundException("Orden no encontrada.");

        return mapper.Map<ViewModel>(entity);
    }
}
