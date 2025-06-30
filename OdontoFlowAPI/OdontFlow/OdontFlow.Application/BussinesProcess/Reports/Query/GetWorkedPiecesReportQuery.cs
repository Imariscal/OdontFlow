using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Common;
using OdontFlow.Domain.Repositories.Base;
using OdontFlow.Domain.ViewModel.Order;
using OdontFlow.Domain.ViewModel.Report;
using Model = OdontFlow.Domain.Entities.StationWork;


namespace OdontFlow.Application.BussinesProcess.Reports.Query;

public class GetWorkedPiecesReportQuery(OrderAdvancedFilterViewModel input)
{
    public OrderAdvancedFilterViewModel Input { get; set; } = input;
}

public class GetWorkedPiecesReportQueryHandler(
    IReadOnlyRepository<Guid, Model> stationWorkRepository,
    IMapper mapper
) : IQueryHandler<GetWorkedPiecesReportQuery, PagedResult<WorkedPiecesReportViewModel>>
{
    public async Task<PagedResult<WorkedPiecesReportViewModel>> Handle(GetWorkedPiecesReportQuery input)
    {
        var query = input.Input;

        var all = await stationWorkRepository.GetAllMatchingAsync(
            sw =>
                sw.WorkStatus == WORK_STATUS.TERMINADO &&
                sw.EmployeeId != null &&
                (!query.CreationDateStart.HasValue || sw.CreationDate >= query.CreationDateStart.Value) &&
                (!query.CreationDateEnd.HasValue || sw.CreationDate <= query.CreationDateEnd.Value) &&
                (string.IsNullOrEmpty(query.Search) ||
                    sw.Employee.Name.Contains(query.Search) ||
                    sw.Order.Barcode.Contains(query.Search)) &&
                (query.ProductIds == null || !query.ProductIds.Any() ||
                    (sw.ProductId.HasValue && query.ProductIds.Contains(sw.ProductId.Value))),
            new[]
            {
                "Employee", "Product", "Order", "Order.Client", "Order.Items", "Order.Client.Employee",
                "Order.Items.Product", "Order.Payments", "WorkStation"
            }
        );

        var workedPieces = all
            .Select(sw =>
            {
                var matchingItem = sw.Order?.Items?.FirstOrDefault(item => item.ProductId == sw.ProductId);

                if (matchingItem == null)
                    return null;

                var unitCost = matchingItem.UnitCost;
                var unitTax = matchingItem.UnitTax;
                var piezas = matchingItem.Teeth?.Count ?? 0;

                return new WorkedPieceViewModel
                {
                    Id = sw.Id,
                    EmployeeName = sw.Employee?.Name ?? "",
                    Order = mapper.Map<OrderViewModel>(sw.Order),
                    ProductName = sw.Product?.Name ?? "",
                    TeethDetails = matchingItem.Teeth?.Select(t => t.ToString()) ?? new List<string>(),
                    EmployeeStartDate = sw.EmployeeStartDate,
                    EmployeeEndDate = sw.EmployeeEndDate,
                    StationName = sw.WorkStation?.Name ?? "",
                    Quantity = piezas,
                    UnitCost = unitCost,
                    Subtotal = unitCost * piezas,
                    TotalTax = unitTax * piezas,
                    Total = (unitCost + unitTax) * piezas
                };
            })
            .Where(x => x != null)
            .OrderBy(x => x!.EmployeeName)
            .ToList()!;

        // Agrupación por producto
        var productGroups = workedPieces
            .GroupBy(p => p.ProductName)
            .Select(g => new ProductPiecesChart
            {
                ProductName = g.Key,
                Pieces = g.Sum(x => x.Quantity.Value)
            })
            .OrderByDescending(x => x.Pieces)
            .ToList();

        // Top 5 productos
        var topProducts = productGroups.Take(5).ToList();

        // Paginado
        var total = workedPieces.Count;
        var skip = (query.Page - 1) * query.PageSize;
        var pagedItems = workedPieces.Skip(skip).Take(query.PageSize).ToList();

        var resumenPorLaboratorista = workedPieces
    .GroupBy(wp => wp.EmployeeName)
    .Select(grupo =>
    {
        var ordenes = grupo
            .Select(wp => wp.Order?.Barcode)
            .Where(barcode => !string.IsNullOrWhiteSpace(barcode))
            .Distinct()
            .Count();

        var productos = grupo
            .GroupBy(wp => wp.ProductName)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(x => x.Quantity ?? 0)
            );

        return new LabTechnicianSummaryViewModel
        {
            Laboratorista = grupo.Key,
            Ordenes = ordenes,
            Productos = productos
        };
    })
    .ToList();

        var result = new WorkedPiecesReportViewModel
        {
            Items = pagedItems,
            TopProducts = topProducts,
            AllProducts = productGroups,
            ConsolidadoPorLaboratorista = resumenPorLaboratorista
        };

        return new PagedResult<WorkedPiecesReportViewModel>(
            new List<WorkedPiecesReportViewModel> { result },
            total,
            query.Page,
            query.PageSize
        );
    }
}

