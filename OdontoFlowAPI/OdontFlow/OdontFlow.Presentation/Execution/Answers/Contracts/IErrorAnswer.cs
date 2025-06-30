namespace OdontFlow.API.Execution.Answers.Contracts;

public interface IErrorAnswer<T> : IAnswerBase<T> where T : class
{
    // URI de un documento que explica el tipo de error.
    string Type { get; }

    // Un resumen legible por humanos del error.
    string Title { get; }

    // El código de estado HTTP generado por el error.
    int Status { get; }

    // Una explicación humana detallada del problema.
    string Detail { get; }

    // Una URI que identifica esta instancia específica del problema.
    string Instance { get; }
}
