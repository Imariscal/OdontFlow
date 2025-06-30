namespace OdontFlow.Domain.ViewModel.StationWork;

public class StationWorkByStationViewModel
{
    public string Cumplimiento { get; set; }
    public string Productividad { get; set; }
    public Guid StationId { get; set; }
    public string StationName { get; set; } = default!;
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
    public int Orden { get; set; }
}
