using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.DTOs;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.User;

namespace OdontFlow.Application.BussinesProcess.Auth.Query
{
    public class GetUserByEmailQuery (LoginRequest input) {
        public LoginRequest Input { get; set; } = input;
    }

    public class GetUserByEmailQueryHandler(IReadOnlyRepository<Guid, Model> repository, IMapper mapper)
    : IQueryHandler<GetUserByEmailQuery, Model>
    {
        public async Task<Model> Handle(GetUserByEmailQuery query)
        {
            var entities = await repository.GetAllMatchingAsync(r => r.Email == query.Input.Email);
            var entity = entities.FirstOrDefault();
            return mapper.Map<Model>(entity); 
        }
    }
}


 