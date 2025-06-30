using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Exceptions;
using OdontFlow.Domain.BusinessRules.Base;
using OdontFlow.Domain.DTOs;
using OdontFlow.Domain.Entities;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.User;

namespace OdontFlow.Application.BussinesProcess.Auth.Command;
public class CreateRegisterCommand(RegisterRequest input) : ICommand<User>
{
    public RegisterRequest Input { get; set; } = input;
}

public class CreateRegisterCommandHandler(
             IWriteOnlyRepository<Guid, Model> writeOnlyRepository,
             IReadOnlyRepository<Guid, Model> readOnlyRepository,
             IMapper mapper,
             IValidationStrategy<RegisterRequest> validationStrategy) : ICommandHandler<CreateRegisterCommand, User>
{
    public async Task<User> Handle(CreateRegisterCommand command)
    {
        ArgumentNullException.ThrowIfNull(command.Input.Email);
        ArgumentNullException.ThrowIfNull(command.Input.Password);
        ArgumentNullException.ThrowIfNull(command);
       
        Validate(command.Input);

        var entityFound = await readOnlyRepository.GetAllMatchingAsync(e => e.Email == command.Input.Email);
        if (entityFound.Any()) throw new Exception("Ya existe un usuario registrado con ese email");

        var entity = mapper.Map<Model>(command.Input);
        entity.PasswordHash = command.Input.Password;
        entity.ChangePassword = true;
        await writeOnlyRepository.AddAsync(entity);

        return mapper.Map<User>(entity);
    }

    private void Validate(RegisterRequest entity)
    {
        var validationResult = validationStrategy.Validate(entity);
        if (!validationResult.IsValid) throw new BusinessValidationException(validationResult.Errors);
    }
}
