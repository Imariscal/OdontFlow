namespace OdontFlow.Domain.ViewModel.StationWork;

public class StationWorkDetailViewModel
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string OrderNumber { get; set; }
    public string Barcode { get; set; }
    public Guid StationWorkId { get; set; }
    public string WorkStationName { get; set; }
    public string ProductName { get; set; } = default!;
    public string OrderColor { get; set; } = default!;
    public string ClientName { get; set; } = default!;
    public List<string> Teeth { get; set; } = new();
    public string TeethDetails
    {
        get
        {
            return Teeth?.Any() == true ? string.Join(", ", Teeth) : string.Empty;
        }
    }

    public string? PreviousStationName { get; set; }
    public string? PreviousEmployeeName { get; set; }
    public DateTime? PreviousEndDate { get; set; }
    public DateTime? StationStartDate { get; set; }
    public DateTime? StationEndDate { get; set; }
    public DateTime? EmployeeStartDate { get; set; }
    public DateTime? EmployeeEndDate { get; set; }
    public int WorkStatusIndicator { get; set; }
    public int WorkStatus { get; set; }
    public string? CurrentEmployee { get; set; }
    public bool InProgress { get; set; }
    public int Step { get; set; }
    public bool WorkedOnTime { get; set; }
    public string ProductivityPercent { get; set; } = default!;
    public string RealTime { get; set; }
    public string EstimatedTime{ get; set; }

    public int? DelayMinutes { get; set; }
}
