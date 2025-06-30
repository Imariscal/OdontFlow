using OdontFlow.API.Execution.Answers.Contracts;

namespace OdontFlow.API.Execution.Answers;

public class SuccessfullyAnswer<T> : ISuccessfullyAnswer<T> where T : class
{
    public SuccessfullyAnswer() { }
    public SuccessfullyAnswer(T payload) { Payload = payload; Message = "Succesfully executed"; }
    public SuccessfullyAnswer(string mensaje) { Message = mensaje; }
    public SuccessfullyAnswer(string messaje, T payload)
    { Message = messaje; Payload = payload; }

    public bool Success => true;

    public string Message { get; set; } = string.Empty;

    public T? Payload { get; set; }
}
