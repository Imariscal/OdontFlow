namespace OdontFlow.Application.Services.Contracts;

public interface IOrderSequenceService
{
    Task<string> GenerateBarcodeAsync();
}
