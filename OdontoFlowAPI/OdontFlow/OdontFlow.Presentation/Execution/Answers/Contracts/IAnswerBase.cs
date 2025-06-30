namespace OdontFlow.API.Execution.Answers.Contracts;

public interface IAnswerBase<T> where T : class
{
    bool Success { get; }
    string Message { get; }
    T? Payload { get; }
}