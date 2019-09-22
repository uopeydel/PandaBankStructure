using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using PandaBank.SharedService.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PandaBank.Account.DAL.Repository.Interface
{
    public interface IGenericEFRepository<CustomDbContext>
    {
        CustomDbContext Context();

        Task<Results<List<TbDest>>> GetPagingAsync<TbSource, TbDest>(
            Expression<Func<TbSource, bool>> predicate,
            Expression<Func<TbSource, TbDest>> selector,
            PagingParameters paging,
            bool? tracking = null,
            bool? lazyLoading = null
            )
            where TbDest : class where TbSource : class;

        Task<CustomTable> FirstOrDefaultAsync<CustomTable>(
            Expression<Func<CustomTable, bool>> predicate, bool AsNoTracking = false)
            where CustomTable : class;

        Task<TSelector> FirstOrDefaultAsync<CustomTable, TSelector>(
            Expression<Func<CustomTable, bool>> predicate,
            Expression<Func<CustomTable, TSelector>> selector,
            bool AsNoTracking = false)
           where CustomTable : class where TSelector : class;

        IQueryable<CustomTable> GetQueryAble<CustomTable>() where CustomTable : class;

        Task<CustomTable> AddAsync<CustomTable>(CustomTable entity)
             where CustomTable : class;

        Task<IEnumerable<CustomTable>> AddRangeAsync<CustomTable>(IEnumerable<CustomTable> entity)
            where CustomTable : class;

        void Update<CustomTable>(CustomTable entity)
           where CustomTable : class;


        void UpdateSpecficProperty<CustomTable, TProperty>(CustomTable entity, params Expression<Func<CustomTable, TProperty>>[] properties)
           where CustomTable : class;

        void UpdateSpecficPropertyMultiType<CustomTable>(CustomTable entity, params Expression<Func<CustomTable, object>>[] properties)
             where CustomTable : class;

        Task<Results<bool>> SaveAsync();

        Task<int> RemoveRange<CustomTable>(List<CustomTable> customTables) where CustomTable : class;
        Task<int> Remove<CustomTable>(CustomTable customTables) where CustomTable : class;

        Task<Results<TResult>> ExecuteWithTransactionAsync<TResult>(Func<Task<TResult>> func);
    }
}
