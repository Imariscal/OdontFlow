using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Common;
using OdontFlow.Domain.DTOs.Order;
using OdontFlow.Domain.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Model = OdontFlow.Domain.Entities.Order;
using ViewModel = OdontFlow.Domain.ViewModel.Order.OrderViewModel;
using System.Linq.Expressions;

namespace OdontFlow.Application.BussinesProcess.Order.Query;

public class GetOrdersQuery(GetPagedOrdersQuery query)
{
    public GetPagedOrdersQuery Query { get; set; } = query;
}

public class GetOrdersQueryHandler(
    IReadOnlyRepository<Guid, Model> readOnlyRepository,
    IMapper mapper
) : IQueryHandler<GetOrdersQuery, PagedResult<ViewModel>>
{
    public async Task<PagedResult<ViewModel>> Handle(GetOrdersQuery request)
    {
        var q = request.Query;
        var skip = (q.Page - 1) * q.PageSize;
        var take = q.PageSize;

        var baseQuery = readOnlyRepository.GetAllMatchingQueryable(
            o => !o.PaymentComplete && !o.Uncollectible,
            "Client",
            "Items", "Items.Product",
            "Payments",
            "StationWorks", "StationWorks.WorkStation", "StationWorks.Employee",
            "Client.Employee"
        );

        // 🔍 Aplicar búsqueda global
        if (!string.IsNullOrWhiteSpace(q.Global))
        {
            var global = q.Global.ToLower();
            baseQuery = baseQuery.Where(o =>
                o.Barcode.ToLower().Contains(global) ||
                o.Client.Name.ToLower().Contains(global) ||
                o.PatientName.ToLower().Contains(global)
            );
        }

        // 🔧 Aplicar filtros dinámicos
        if (q.Filters != null)
        {
            foreach (var (field, filterMeta) in q.Filters)
            {
                var value = filterMeta.Value;
                if (string.IsNullOrWhiteSpace(value)) continue;

                switch (field)
                {
                    case "barcode":
                        baseQuery = baseQuery.Where(o => o.Barcode.Contains(value));
                        break;

                    case "clientName":
                        baseQuery = baseQuery.Where(o => o.Client.Name.Contains(value));
                        break;

                    case "patientName":
                        baseQuery = baseQuery.Where(o => o.PatientName.Contains(value));
                        break;

                    //case "orderStatus":
                    //    baseQuery = baseQuery.Where(o => o.OrderStatusId == value);
                    //    break;

                    //case "workGroup":
                    //    baseQuery = baseQuery.Where(o => o.Client.GroupId == W);
                    //    break;

                    case "paymentComplete":
                        if (bool.TryParse(value, out var pc))
                            baseQuery = baseQuery.Where(o => o.PaymentComplete == pc);
                        break;

                    case "applyInvoice":
                        if (bool.TryParse(value, out var invoice))
                            baseQuery = baseQuery.Where(o => o.ApplyInvoice == invoice);
                        break;

                    //case "orderType":
                    //    baseQuery = baseQuery.Where(o => o.OrderType == value);
                    //    break;

                    case "creationDate":
                        if (DateTime.TryParse(value, out var cd))
                            baseQuery = baseQuery.Where(o => o.CreationDate.Date == cd.Date);
                        break;

                    case "commitmentDate":
                        if (DateTime.TryParse(value, out var cmp))
                            baseQuery = baseQuery.Where(o => o.CommitmentDate.Date == cmp.Date);
                        break;

                    case "processDate":
                        if (DateTime.TryParse(value, out var prd))
                            baseQuery = baseQuery.Where(o => o.CreationDate == prd.Date);
                        break;

                    case "paymentDate":
                        if (DateTime.TryParse(value, out var payd))
                            baseQuery = baseQuery.Where(o => o.PaymentDate.HasValue && o.PaymentDate.Value.Date == payd.Date);
                        break;
                }
            }
        }

        // 🔽 Ordenamiento dinámico
        if (!string.IsNullOrWhiteSpace(q.SortField))
        {
            baseQuery = q.SortOrder == 1
                ? baseQuery.OrderByDynamic(q.SortField)
                : baseQuery.OrderByDescendingDynamic(q.SortField);
        }
        else
        {
            baseQuery = baseQuery.OrderByDescending(o => o.CreationDate);
        }

        var totalCount = await baseQuery.CountAsync();

        var pagedEntities = await baseQuery
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        var viewModels = mapper.Map<List<ViewModel>>(pagedEntities);

        return new PagedResult<ViewModel>
        {
            Items = viewModels,
            TotalCount = totalCount
        };
    }
}


public static class QueryableExtensions
{
    public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string propertyName)
    {
        return query.OrderBy(ToLambda<T>(propertyName));
    }

    public static IQueryable<T> OrderByDescendingDynamic<T>(this IQueryable<T> query, string propertyName)
    {
        return query.OrderByDescending(ToLambda<T>(propertyName));
    }

    private static Expression<Func<T, object>> ToLambda<T>(string propertyName)
    {
        var param = Expression.Parameter(typeof(T), "x");
        Expression body = propertyName.Split('.').Aggregate<string, Expression>(param, Expression.PropertyOrField);
        return Expression.Lambda<Func<T, object>>(Expression.Convert(body, typeof(object)), param);
    }
}

