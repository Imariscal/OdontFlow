using Mapster;
using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Exceptions;
using OdontFlow.Domain.BusinessRules.Base;
using OdontFlow.Domain.DTOs;
using OdontFlow.Domain.Entities;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.User;

namespace OdontFlow.Application.BussinesProcess.Auth.Command;

public class ChangeFirstTimePasswordCommand(Guid userId, ResetFirstPasswordRequest input) : ICommand<User>
{
    public Guid UserId { get; set; } = userId;
    public ResetFirstPasswordRequest Input { get; set; } = input;
}
public class ChangeFirstTimePasswordCommandHandler(
             IWriteOnlyRepository<Guid, Model> writeOnlyRepository,
             IReadOnlyRepository<Guid, Model> readOnlyRepository,
             IMapper mapper,
             IValidationStrategy<ResetFirstPasswordRequest> validationStrategy) : ICommandHandler<ChangeFirstTimePasswordCommand, User>
{
    public async Task<User> Handle(ChangeFirstTimePasswordCommand command)
    { 
        ArgumentNullException.ThrowIfNull(command.Input.NewPassword);
        ArgumentNullException.ThrowIfNull(command);

        Validate(command.Input);

        var entityFound = await readOnlyRepository.GetAsync(command.UserId);
        if (entityFound == null) throw new Exception("El usuario no registrado esta registrado en el sistema");
        entityFound.PasswordHash = command.Input.NewPassword;
        entityFound.ChangePassword = false;
        await writeOnlyRepository.Modify(entityFound);

        return entityFound;
    }

    private void Validate(ResetFirstPasswordRequest entity)
    {
        var validationResult = validationStrategy.Validate(entity);
        if (!validationResult.IsValid) throw new BusinessValidationException(validationResult.Errors);
    }
}