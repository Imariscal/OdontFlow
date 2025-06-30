using OdontFlow.API.Execution.Answers.Contracts;
using OdontFlow.API.Execution.Answers;
using OdontFlow.CrossCutting.Exceptions;

namespace OdontFlow.API.Execution;

public class SafeExecutor<TObjectResult> where TObjectResult : class
{
    public static IAnswerBase<TObjectResult> Exec(Func<TObjectResult> predicate)
    {
        try
        {
            var response = predicate();

            return response is null ? throw new NotFoundException() : new SuccessfullyAnswer<TObjectResult>(response);
        }
        catch (Exception ex)
        {
            GC.SuppressFinalize(predicate);

            var message = ex.Message;
            if (ex.InnerException != null)
            {
                message += $" | InnerException: {ex.InnerException.Message}";
            }

            var enhancedException = new Exception(message, ex);
            return new ErrorAnswer<TObjectResult>(enhancedException);
        }
    }

    public static async Task<IAnswerBase<TObjectResult>> ExecAsync(Func<Task<TObjectResult>> predicate)
    {
        try
        {
            var response = await predicate().ConfigureAwait(false);
            return response is null
                ? throw new NotFoundException("The item was not found in the database")
                : new SuccessfullyAnswer<TObjectResult>(response);
        }
        catch (Exception ex)
        {
            GC.SuppressFinalize(predicate);

            var message = ex.Message;
            if (ex.InnerException != null)
            {
                message += $" | InnerException: {ex.InnerException.Message}";
            }

            var enhancedException = new Exception(message, ex);
            return new ErrorAnswer<TObjectResult>(enhancedException);
        }
    }
}

public class SafeExecutor
{
    public static IAnswerBase<object> Exec(Func<object> predicate)
    {
        try
        {
            var response = predicate();
            return new SuccessfullyAnswer<object>(response);
        }
        catch (Exception ex)
        {
            GC.SuppressFinalize(predicate);

            var message = ex.Message;
            if (ex.InnerException != null)
            {
                message += $" | InnerException: {ex.InnerException.Message}";
            }

            var enhancedException = new Exception(message, ex);
            return new ErrorAnswer<object>(enhancedException);
        }
    }

    public static async Task<IAnswerBase<Task>> ExecAsync(Func<Task> predicate)
    {
        try
        {
            await predicate();
            return new SuccessfullyAnswer<Task>();
        }
        catch (Exception ex)
        {
            GC.SuppressFinalize(predicate);

            var message = ex.Message;
            if (ex.InnerException != null)
            {
                message += $" | InnerException: {ex.InnerException.Message}";
            }

            var enhancedException = new Exception(message, ex);
            return new ErrorAnswer<Task>(enhancedException);            
        }
    }
}
