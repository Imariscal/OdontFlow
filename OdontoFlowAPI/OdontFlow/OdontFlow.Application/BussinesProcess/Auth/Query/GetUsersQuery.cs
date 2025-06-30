using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base;
using OdontFlow.Domain.ViewModel.User;
using Model = OdontFlow.Domain.Entities.User;

namespace OdontFlow.Application.BussinesProcess.Auth.Query;

public class GetUsersQuery
{
}

public class GetUsersQueryHandler(IReadOnlyRepository<Guid, Model> repository, IMapper mapper)
    : IQueryHandler<GetUsersQuery, IEnumerable<UserViewModel>>
{
    public async Task<IEnumerable<UserViewModel>> Handle(GetUsersQuery query)
    {
        var entities = await repository.GetAllAsync(new[] { "Employee", "Client" });
        return mapper.Map<IEnumerable<UserViewModel>>(entities);
    }
}
