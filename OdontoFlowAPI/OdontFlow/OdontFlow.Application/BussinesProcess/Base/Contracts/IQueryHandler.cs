namespace OdontFlow.Application.BussinesProcess.Base.Contracts;

public interface IQueryHandler<TQuery, TResult>
{
    Task<TResult> Handle(TQuery query);
}