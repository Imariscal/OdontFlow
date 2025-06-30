using OdontFlow.Domain.ViewModel.StationWork;

namespace OdontFlow.Domain.ViewModel.Lab;

public class StationWorkSummaryViewModel
{
    public double Productividad { get; set; }
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
    public List<StationWorkByStationViewModel> PorEstacion { get; set; } = new();

    public List<StationWorkByEmployeeViewModel> PorEmpleado { get; set; } = new();
}
