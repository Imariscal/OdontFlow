 
namespace OdontFlow.Domain.ViewModel.Report;

public class PagedWithSummaryResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }

    public DebtSummaryViewModel Summary { get; set; } = new();
}
