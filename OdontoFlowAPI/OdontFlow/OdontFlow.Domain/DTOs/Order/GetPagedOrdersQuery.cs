using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdontFlow.Domain.DTOs.Order;

public class GetPagedOrdersQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
 
    public string? SortField { get; set; }
    public int? SortOrder { get; set; } // 1 = ASC, -1 = DESC

    public string? Global { get; set; }
    public Dictionary<string, FilterMetadata>? Filters { get; set; }
}

public class FilterMetadata
{
    public string? MatchMode { get; set; }
    public string? Value { get; set; }
}