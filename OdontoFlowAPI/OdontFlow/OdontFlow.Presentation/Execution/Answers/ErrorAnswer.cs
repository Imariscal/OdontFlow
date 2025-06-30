using OdontFlow.API.Execution.Answers.Contracts;
using OdontFlow.CrossCutting.Exceptions;

namespace OdontFlow.API.Execution.Answers;

public class ErrorAnswer<T> : IErrorAnswer<T> where T : class
{
    public ErrorAnswer(string message, string type, string title, int status, string detail, string instance, T? payload = null)
    {
        Message = message;
        Type = type;
        Title = title;
        Status = status;
        Detail = detail;
        Instance = instance;
        Payload = payload;
    }

    public ErrorAnswer(Exception ex)
    {
        Errors = ex is BusinessValidationException exception
            ? exception.Errors
            : new Dictionary<string, string[]> { { "Error", new string[] { ex.Message } } };
    }

    public Dictionary<string, string[]> Errors { get; } = [];

    public bool Success { get; private set; } = false;
    public string Message { get; private set; } = string.Empty;
    public T? Payload { get; private set; }
    public string Type { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public int Status { get; private set; }
    public string Detail { get; private set; } = string.Empty;
    public string Instance { get; private set; } = string.Empty;
}
