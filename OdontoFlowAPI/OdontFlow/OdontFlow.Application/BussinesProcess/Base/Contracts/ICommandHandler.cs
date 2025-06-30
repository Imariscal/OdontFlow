namespace OdontFlow.Application.BussinesProcess.Base.Contracts;

public interface ICommandHandler<TCommand, TResult> where TCommand : class, ICommand<TResult>
{
    Task<TResult> Handle(TCommand command);
}

public interface ICommandHandler<TCommand> where TCommand : class
{
    Task Handle(TCommand command);
}