using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using PandaBank.SharedService.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PandaBank.Account.DAL.Repository.Interface;

namespace PandaBank.Account.DAL.Repository.Implement
{
    public class GenericEFRepository<CustomDbContext> : IGenericEFRepository<CustomDbContext>
       where CustomDbContext : DbContext
    {
        private readonly CustomDbContext _msGenericDb;
        public CustomDbContext Context() { return _msGenericDb; }
        public GenericEFRepository(CustomDbContext msGenericDb)
        {
            _msGenericDb = msGenericDb;
        }

        public async Task<Results<List<TbDest>>> GetPagingAsync<TbSource, TbDest>(
           Expression<Func<TbSource, bool>> predicate,
           Expression<Func<TbSource, TbDest>> selector,
           PagingParameters paging,
           bool? tracking = null,
           bool? lazyLoading = null) where TbDest : class where TbSource : class
        {
            if (lazyLoading == false)
            {
                _msGenericDb.ChangeTracker.LazyLoadingEnabled = false;
            }

            var query = _msGenericDb.Set<TbSource>().Where(predicate).Select(selector);
            var results = await QueryExt.PagingResultProjectionAsync<IQueryable<TbDest>, TbDest>(query, paging);
            if (results.IsError())
            {
                var err = PandaResponse.CreateErrorResponse<List<TbDest>>("");
                err.Errors = results.Errors.ToList();
                return err;
            }
            List<TbDest> result;
            if (tracking == false)
            {
                result = await results.Data?.AsNoTracking().ToListAsync();
            }
            else
            {
                result = await results.Data?.ToListAsync();
            }

            var response = PandaResponse.CreateSuccessResponse(result, results.PageInfo);
            return response;

        }

        public async Task<CustomTable> FirstOrDefaultAsync<CustomTable>(
            Expression<Func<CustomTable, bool>> predicate,
            bool AsNoTracking = false)
            where CustomTable : class
        {
            var query = _msGenericDb.Set<CustomTable>().Where(predicate);
            if (AsNoTracking)
            {
                query = query.AsNoTracking();
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<TSelector> FirstOrDefaultAsync<CustomTable, TSelector>(
            Expression<Func<CustomTable, bool>> predicate,
            Expression<Func<CustomTable, TSelector>> selector,
            bool AsNoTracking = false)
           where CustomTable : class where TSelector : class
        {
            var query = _msGenericDb.Set<CustomTable>().Where(predicate).Select(selector);
            if (AsNoTracking)
            {
                query = query.AsNoTracking();
            }
            return await query.FirstOrDefaultAsync();
        }

        public IQueryable<CustomTable> GetQueryAble<CustomTable>() where CustomTable : class
        {
            return _msGenericDb.Set<CustomTable>();
        }

        public async Task<CustomTable> AddAsync<CustomTable>(CustomTable entity)
            where CustomTable : class
        {
            await _msGenericDb.Set<CustomTable>().AddAsync(entity);
            return entity;
        }

        public async Task<IEnumerable<CustomTable>> AddRangeAsync<CustomTable>(IEnumerable<CustomTable> entity)
            where CustomTable : class
        {
            var addAsync = entity as CustomTable[] ?? entity.ToArray();
            await _msGenericDb.Set<CustomTable>().AddRangeAsync(addAsync);
            return addAsync;
        }

        public void Update<CustomTable>(CustomTable entity)
            where CustomTable : class
        {
            _msGenericDb.Entry(entity).State = EntityState.Modified;
        }

        public void UpdateSpecficProperty<CustomTable, TProperty>(CustomTable entity, params Expression<Func<CustomTable, TProperty>>[] properties)
            where CustomTable : class
        {
            if (properties != null && properties.Length > 0)
            {
                _msGenericDb.Entry(entity).State = EntityState.Detached;
                _msGenericDb.Attach(entity);
                foreach (var prop in properties)
                {
                    _msGenericDb.Entry(entity).Property(prop).IsModified = true;
                }
                return;
            }
            _msGenericDb.Entry(entity).State = EntityState.Modified;

        }

        public void UpdateSpecficPropertyMultiType<CustomTable>(CustomTable entity, params Expression<Func<CustomTable, object>>[] properties)
             where CustomTable : class
        {
            if (properties != null && properties.Length > 0)
            {
                _msGenericDb.Entry(entity).State = EntityState.Detached;
                _msGenericDb.Attach(entity);
                foreach (var prop in properties)
                {
                    _msGenericDb.Entry(entity).Property(prop).IsModified = true;
                }
                return;
            }
            _msGenericDb.Entry(entity).State = EntityState.Modified;
        }

        public async Task<Results<bool>> SaveAsync()
        {
            try
            {
                var result = await _msGenericDb.SaveChangesAsync();
                return PandaResponse.CreateSuccessResponse(result > 0);
            }
            catch (Exception ex)
            {
                var tran = _msGenericDb.Database.CurrentTransaction;
                if (tran != null)
                {
                    tran.Rollback();
                }
                return PandaResponse.CreateErrorResponse<bool>(ex.Message, ex);
            }
        }

        private string GetAllMessages(Exception ex, string separator = "\r\nInnerException: ")
        {
            if (ex.InnerException == null)
            {
                return ex.Message;
            }

            return ex.Message + separator + GetAllMessages(ex.InnerException, separator);
        }

        public async Task<int> RemoveRange<CustomTable>(List<CustomTable> customTables) where CustomTable : class
        {
            _msGenericDb.Set<CustomTable>().RemoveRange(customTables);
            return await _msGenericDb.SaveChangesAsync();
        }

        public async Task<int> Remove<CustomTable>(CustomTable customTables) where CustomTable : class
        {
            _msGenericDb.Set<CustomTable>().Remove(customTables);
            return await _msGenericDb.SaveChangesAsync();
        }

        public virtual async Task<Results<TResult>> ExecuteWithTransactionAsync<TResult>(Func<Task<TResult>> func)
        {
            using (var t = _msGenericDb.Database.BeginTransaction())
            {
                try
                {
                    var result = await func.Invoke();
                    t.Commit();
                    return new Results<TResult>()
                    {
                        Data = result
                    };
                }
                catch (Exception e)
                {
                    if (t != null)
                    {
                        t.Rollback();
                    }
                    return PandaResponse.CreateErrorResponse<TResult>(GetAllMessages(e));
                }
            }
        }
    }
}
