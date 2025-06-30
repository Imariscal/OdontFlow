using OdontFlow.Domain.ViewModel.Order;
 
namespace OdontFlow.Domain.ViewModel.Report;

public class WorkedPiecesReportViewModel
{
    public List<WorkedPieceViewModel> Items { get; set; } = [];
    public List<ProductPiecesChart> TopProducts { get; set; } = [];
    public List<ProductPiecesChart> AllProducts { get; set; } = [];

    public List<LabTechnicianSummaryViewModel> ConsolidadoPorLaboratorista { get; set; } = new();
}

public class LabTechnicianSummaryViewModel
{
    public string Laboratorista { get; set; } = default!;
    public int Ordenes { get; set; }
    public Dictionary<string, int> Productos { get; set; } = new();
}

public class ProductPiecesChart
{
    public string ProductName { get; set; } = string.Empty;
    public int Pieces { get; set; }
}
public class WorkedPieceViewModel
{
    public Guid Id { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public OrderViewModel Order { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public IEnumerable<string> TeethDetails { get; set; } = new List<string>();
    public DateTime EmployeeStartDate { get; set; }
    public DateTime EmployeeEndDate { get; set; }
    public string StationName { get; set; } = string.Empty;
    public int? Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TotalTax { get; set; }
    public decimal Total { get; set; } 
}
 




