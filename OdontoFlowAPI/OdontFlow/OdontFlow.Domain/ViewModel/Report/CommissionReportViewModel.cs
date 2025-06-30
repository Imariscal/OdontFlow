using OdontFlow.Domain.ViewModel.Order;

public class CommissionOrdersReportViewModel
{
    public List<CommissionOrderDetailItem> Items { get; set; } = [];

    public List<CommissionEmployeeSummary> Summary { get; set; } = [];
    public int TotalRecords { get; set; }
}

public class CommissionOrderDetailItem
{
    public string EmployeeName { get; set; } = default!;
    public string OrderBarcode { get; set; } = default!;
    public string ClientName { get; set; } = default!;
    public DateTime CreationDate { get; set; }
    public decimal OrderTotal { get; set; }
    public decimal CommissionPercentage { get; set; }
    public decimal CommissionAmount { get; set; }

    public OrderViewModel Order { get; set; }
}


public class CommissionEmployeeSummary
{
    public string EmployeeName { get; set; } = default!;
    public int TotalOrders { get; set; }
    public decimal TotalCommission { get; set; }
    public decimal TotalAmount { get; set; }
}