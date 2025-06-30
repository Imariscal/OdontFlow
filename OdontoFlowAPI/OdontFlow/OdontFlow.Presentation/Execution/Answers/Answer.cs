using OdontFlow.API.Execution.Answers.Contracts;

namespace OdontFlow.API.Execution;

public class Answer<T> : IAnswerBase<T> where T : class
{
    public bool Success { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public T? Payload { get; private set; }

    public static Answer<T> SuccessResult(T payload) =>
        new() { Success = true, Payload = payload };

    public static Answer<T> Failure(string message) =>
        new() { Success = false, Message = message };
}
