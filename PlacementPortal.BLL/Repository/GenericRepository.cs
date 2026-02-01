using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PlacementPortal.BLL.Constants;
using PlacementPortal.DAL.Data;
using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.BLL;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly PlacementPortalDbContext _context;

    private readonly DbSet<T> _dbSet;

    public GenericRepository(PlacementPortalDbContext placementPortalDbContext)
    {
        _context = placementPortalDbContext;
        _dbSet = _context.Set<T>();

    }

    public async Task<GenericReturnDTO> Add(T entity)
    {
        try
        {
            await _dbSet.AddAsync(entity);
            // await _context.SaveChangesAsync();

            return new GenericReturnDTO
            {
                Success = true,
                Message = "Entity added successfully.",
                Data = entity
            };
        }
        catch (Exception ex)
        {
            return new GenericReturnDTO
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public GenericReturnDTO Update(T entity)
    {
        try
        {
            _dbSet.Update(entity);
            // _context.SaveChangesAsync();

            return new GenericReturnDTO
            {
                Success = true,
                Message = "Entity Updated Successfully",
                Data = entity
            };
        }
        catch (Exception Ex)
        {
            return new GenericReturnDTO
            {
                Success = false,
                Message = Ex.Message
            };
        }
    }

    public GenericReturnDTO Remove(T entity)
    {
        try
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
            return new GenericReturnDTO
            {
                Success = true,
                Message = "Entity Removed Successfully",
                Data = entity
            };
        }
        catch (Exception Ex)
        {
            return new GenericReturnDTO
            {
                Success = false,
                Message = Ex.Message
            };
        }
    }

    public async Task<GenericReturnDTO> AddRange(List<T> entities)
    {
        try
        {
            await _dbSet.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
            return new GenericReturnDTO
            {
                Success = true,
                Message = "Entities Added Successfully",
                Data = entities
            };
        }
        catch (Exception Ex)
        {
            return new GenericReturnDTO
            {
                Success = false,
                Message = Ex.Message
            };
        }
    }

    public GenericReturnDTO UpdateRange(List<T> entities)
    {
        try
        {
            _dbSet.UpdateRange(entities);
            _context.SaveChanges();
            return new GenericReturnDTO
            {
                Success = true,
                Message = "Entities Added Successfully",
                Data = entities
            };
        }
        catch (Exception Ex)
        {
            return new GenericReturnDTO
            {
                Success = false,
                Message = Ex.Message
            };
        }
    }

    public async Task<T> GetFirstOrDefault(Expression<Func<T, bool>> expression)
    {
        try
        {
            return await _dbSet.Where(expression).FirstOrDefaultAsync();
        }
        catch (Exception)
        {
            throw new Exception(CustomExceptionConst.DataNotFound);
        }


    }

    public async Task<bool> Exists(Expression<Func<T, bool>> expression)
    {
        return await _dbSet.AnyAsync(expression);
    }

    public async Task<List<T>> GetList(Expression<Func<T, bool>> expression)
    {
        return await _dbSet.Where(expression).ToListAsync();
    }

    // public async Task<List<TResult>> GetDistinctValues<TResult>(Expression<Func<T, bool>> expression, Expression<Func<T, object>> DistinctBy, Expression<Func<T, TResult>> selectExpression)
    // {
    //     return await _dbSet.Where(expression).DistinctBy(DistinctBy).Select(selectExpression).ToListAsync();
    // }



    public async Task<List<TResult>> GetDistinctValues<TResult>(
    Expression<Func<T, bool>> expression,
    Expression<Func<T, object>> groupByExpression,
    Expression<Func<IGrouping<object, T>, TResult>> selectExpression)
    {
        return await _dbSet
            .Where(expression)
            .GroupBy(groupByExpression)
            .Select(selectExpression)
            .ToListAsync();
    }




    public async Task<List<T>> GetListInclude(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.Where(expression).ToListAsync();
    }


    public async Task<List<TResult>> GetListDesired<TResult>(Expression<Func<T, bool>> expression, Expression<Func<T, TResult>> selectExpression)
    {
        return await _dbSet.Where(expression).Select(selectExpression).ToListAsync();
    }


    public IQueryable<T> GetQueryable(Expression<Func<T, bool>> expression)
    {
        return _dbSet.Where(expression).AsQueryable();
    }

    public async Task<int> Count(Expression<Func<T, bool>> expression)
    {
        try
        {
            var list = await _dbSet.Where(expression).ToListAsync();
            return list.Count;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<(int, List<TResult>)> GetListAllDTOs<TResult>(
        Expression<Func<T, bool>> expression,
        Expression<Func<T, TResult>> selector
    )
    {
        try
        {
            IQueryable<T> query = _dbSet.Where(expression);
            List<TResult> list = await query.Select(selector).ToListAsync();

            return (list.Count, list);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<(int, List<TResult>)> GetListPaginated<TResult>(
    Expression<Func<T, bool>> expression,
    Expression<Func<T, TResult>> selector,
    Expression<Func<T, object>> orderBy,
    Expression<Func<T, object>> orderThenBy,
    bool sortOrder,
    int currentPage,
    int pageSize
    )
    {
        try
        {
            int skip = (currentPage - 1) * pageSize;
            IQueryable<T> query = _dbSet.Where(expression);

            query = sortOrder
                ? query.OrderBy(orderBy).ThenBy(orderThenBy)
                : query.OrderByDescending(orderBy).ThenBy(orderThenBy);

            var list = await query.Select(selector).Skip(skip).Take(pageSize).ToListAsync();

            return (_dbSet.Count(expression), list);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async virtual Task<T> GetFirstOrDefaultInclude(
        Expression<Func<T, bool>> filter,
        params Expression<Func<T, object>>[] includes
    )
    {
        try
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query.FirstOrDefaultAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }
}