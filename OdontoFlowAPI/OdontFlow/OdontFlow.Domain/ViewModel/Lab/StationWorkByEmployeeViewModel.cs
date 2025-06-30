
namespace OdontFlow.Domain.ViewModel.Lab;

public class StationWorkByEmployeeViewModel
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;

    public int TotalTrabajos { get; set; }
    public int TrabajosEnEspera { get; set; }
    public int TrabajosEnProceso { get; set; }
    public int TrabajosTerminados { get; set; }
    public int TrabajosBloqueados { get; set; }
    public int TrabajosRechazados { get; set; }

    public int TrabajosEnTiempo { get; set; }
    public int TrabajosConAlarma { get; set; }
    public int TrabajosConRetraso { get; set; }

    public int TrabajosTerminadosHoy { get; set; }
    public int TrabajosComprometidosHoy { get; set; }
    public string Cumplimiento { get; set; }
    public string Productividad { get; set; }

}
