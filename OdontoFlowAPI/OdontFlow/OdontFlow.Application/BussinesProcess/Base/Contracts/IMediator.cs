namespace OdontFlow.Application.BussinesProcess.Base.Contracts;
public interface IMediator
{
    ICommandHandler<TCommand, TResult> GetCommandHandler<TCommand, TResult>() where TCommand : class, ICommand<TResult>;
    ICommandHandler<TCommand> GetCommandHandler<TCommand>() where TCommand : class;
    IQueryHandler<TQuery, TResult> GetQueryHandler<TQuery, TResult>();
}