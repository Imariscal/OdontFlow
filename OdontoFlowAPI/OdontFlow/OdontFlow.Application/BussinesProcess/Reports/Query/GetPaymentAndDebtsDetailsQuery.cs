using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Common;
using OdontFlow.Domain.Repositories.Base;
using OdontFlow.Domain.ViewModel.Order;
using OdontFlow.Domain.ViewModel.OrderPayment;
using System.Linq;
using System.Linq.Expressions;
using Model = OdontFlow.Domain.Entities.Order;
using PaymentModel = OdontFlow.Domain.Entities.OrderPayment;

namespace OdontFlow.Application.BussinesProcess.Reports.Query;
public class GetPaymentAndDebtsDetailsQuery(OrderAdvancedFilterViewModel input)
{
    public OrderAdvancedFilterViewModel Input { get; set; } = input;
}

public class GetPaymentAndDebtsDetailsQueryHandler(
    IReadOnlyRepository<Guid, Model> readOnlyRepository,
    IReadOnlyRepository<Guid, PaymentModel> readOnlyPaymentRepository,
    IMapper mapper
) : IQueryHandler<GetPaymentAndDebtsDetailsQuery, PagedResult<ClientOrdersSummaryViewModel>>
{
    public async Task<PagedResult<ClientOrdersSummaryViewModel>> Handle(GetPaymentAndDebtsDetailsQuery queryData)
    {
        var input = queryData.Input;
        var search = input.Search?.ToLower()?.Trim();

        // 🔹 1. Filtro para pagos
        var paidFilter = PredicateBuilder.True<PaymentModel>();
        paidFilter = paidFilter.And(x =>
            (!input.CreationDateStart.HasValue || x.CreationDate >= input.CreationDateStart.Value) &&
            (!input.CreationDateEnd.HasValue || x.CreationDate <= input.CreationDateEnd.Value));

        if (!string.IsNullOrWhiteSpace(search))
        {
            paidFilter = paidFilter.And(x =>
                x.Order.PatientName.ToLower().Contains(search) ||
                x.Order.RequesterName.ToLower().Contains(search) ||
                x.Order.Barcode.ToLower().Contains(search) ||
                x.Order.Client!.Name.ToLower().Contains(search));
        }

        var payments = await readOnlyPaymentRepository.GetAllMatchingAsync(
            paidFilter,
            "Order", "Order.Client"
        );

        // 🔹 2. Filtro para adeudos
        var debtFilter = PredicateBuilder.True<Model>();
        debtFilter = debtFilter.And(x =>
            x.PaymentComplete == false &&
            x.OrderStatusId == ORDER_STATUS.ENTREGADO &&
            (x.Uncollectible == null || x.Uncollectible == false));

        if (!string.IsNullOrWhiteSpace(search))
        {
            debtFilter = debtFilter.And(x =>
                x.PatientName.ToLower().Contains(search) ||
                x.RequesterName.ToLower().Contains(search) ||
                x.Barcode.ToLower().Contains(search) ||
                x.Client!.Name.ToLower().Contains(search));
        }

        var debts = await readOnlyRepository.GetAllMatchingAsync(
            debtFilter,
            "Client", "Items", "Payments", "StationWorks", "StationWorks.WorkStation", "StationWorks.Employee", "Items.Product", "Client.Employee"
        );

        // 🔹 3. Agrupación por cliente → paciente
        var grouped = payments
            .Select(p => new { ClientName = p.Order.Client!.Name, PatientName = p.Order.PatientName })
            .Union(debts.Select(d => new { ClientName = d.Client!.Name, PatientName = d.PatientName }))
            .Distinct()
            .GroupBy(x => x.ClientName)
            .Select(clientGroup => new ClientOrdersSummaryViewModel
            {
                ClientName = clientGroup.Key,
                Patients = clientGroup
                    .GroupBy(g => g.PatientName)
                    .Select(patientGroup =>
                    {
                        var patientName = patientGroup.Key;

                        var patientPayments = payments
                            .Where(p => p.Order.Client!.Name == clientGroup.Key && p.Order.PatientName == patientName)
                            .ToList();

                        var patientDebts = debts
                            .Where(d => d.Client!.Name == clientGroup.Key && d.PatientName == patientName)
                            .ToList();

                        return new PatientOrdersSummaryViewModel
                        {
                            PatientName = patientName,
                            PaidCount = patientPayments.Count,
                            DebtCount = patientDebts.Count,
                            Payments = mapper.Map<List<OrderPaymentViewModel>>(patientPayments),
                            DebtOrders = mapper.Map<List<OrderViewModel>>(patientDebts)
                        };
                    }).ToList()
            }).ToList();

        // 🔹 4. Paginación por cliente
        var total = grouped.Count;
        var skip = (input.Page - 1) * input.PageSize;
        var paged = grouped
            .OrderBy(x => x.ClientName)
            .Skip(skip)
            .Take(input.PageSize)
            .ToList();

        return new PagedResult<ClientOrdersSummaryViewModel>(
            paged,
            total,
            input.Page,
            input.PageSize
        );
    }
}

public static class PredicateBuilder
{
    public static Expression<Func<T, bool>> True<T>() => x => true;
    public static Expression<Func<T, bool>> False<T>() => x => false;

    public static Expression<Func<T, bool>> And<T>(
        this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var param = Expression.Parameter(typeof(T));
        var body = Expression.AndAlso(
            Expression.Invoke(expr1, param),
            Expression.Invoke(expr2, param)
        );
        return Expression.Lambda<Func<T, bool>>(body, param);
    }

    public static Expression<Func<T, bool>> Or<T>(
        this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var param = Expression.Parameter(typeof(T));
        var body = Expression.OrElse(
            Expression.Invoke(expr1, param),
            Expression.Invoke(expr2, param)
        );
        return Expression.Lambda<Func<T, bool>>(body, param);
    }
}

