using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PandaBank.SharedService.Extensions
{
    public static class QueryExt
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source,
                        bool asc, params string[] orderByProperties) where T : class
        {
            var command = asc ? "OrderBy" : "OrderByDescending";
            var type = typeof(T);
            var parameter = Expression.Parameter(type, "p");

            var parts = orderByProperties[0].Split('.');

            Expression parent = parameter;

            foreach (var part in parts)
            {
                parent = Expression.Property(parent, part);
            }
            var orderByExpression = Expression.Lambda(parent, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new[] { type, parent.Type },
                source.Expression, Expression.Quote(orderByExpression));
            return source.Provider.CreateQuery<T>(resultExpression);
        }
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static async Task<Results<IQueryable<O>>> PagingResultProjectionAsync<DTO, O>
            (IQueryable<O> query, PagingParameters paging, bool noLimit = false)
            where O : class
        {
            try
            {
                query = QueryExt.OrderBy(query, paging.Asc, paging.OrderBy);
                paging.Top = (paging.Top < 0 || paging.Top > 5) ? 5 : paging.Top;
                int total = 0;
                if (paging.Top != 0)
                {
                    paging.PageSize = paging.Top;
                    paging.Page = 1;
                    total = paging.Top;
                    query = query
                        .Take(paging.Top);
                }
                else if (noLimit == false)
                {
                    paging.PageSize = (paging.PageSize > 51 || paging.PageSize < 1) ? 20 : paging.PageSize;
                    paging.Page = (paging.Page < 1) ? 1 : paging.Page;
                    total = await query.CountAsync();
                    query = query
                        .Skip(paging.PageSize * (paging.Page - 1))
                        .Take(paging.PageSize);
                }
                else
                {
                    total = await query.CountAsync();
                    paging.PageSize = total;
                    paging.Page = 1;
                }

                var totalPages = paging.PageSize > 0 ? (int)Math.Ceiling((double)total / paging.PageSize) : 0;
                var pageInfo = new PagingInfo
                {
                    Total = total,
                    TotalPages = totalPages,
                    CurrentPage = paging.Page,
                    PageSize = paging.PageSize
                };

                return PandaResponse.CreateSuccessResponse<IQueryable<O>>(query, pageInfo);
            }
            catch (Exception ex)
            {
                var result = new Results<List<DTO>>();
                result.Errors = new List<string> { ex.Message, ex.InnerException?.Message, ex.InnerException?.InnerException?.Message };
                return PandaResponse.CreateErrorResponse<IQueryable<O>>("error while get paging", ex);
            }
        }
    }
}
