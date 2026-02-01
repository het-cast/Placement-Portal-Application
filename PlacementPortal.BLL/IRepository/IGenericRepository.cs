using System.Linq.Expressions;
using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.BLL;

public interface IGenericRepository<T> where T : class
{
    Task<GenericReturnDTO> Add(T entity);

    GenericReturnDTO Update(T entity);

    GenericReturnDTO Remove(T entity);

    Task<T>? GetFirstOrDefault(Expression<Func<T, bool>> expression);

    Task<bool> Exists(Expression<Func<T, bool>> expression);

    // Retrieving List without pagination
    Task<List<T>> GetList(Expression<Func<T, bool>> expression);

    Task<List<T>> GetListInclude(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes);

    IQueryable<T> GetQueryable(Expression<Func<T, bool>> expression);

    Task<GenericReturnDTO> AddRange(List<T> entity);

    GenericReturnDTO UpdateRange(List<T> entities);

    Task<int> Count(Expression<Func<T, bool>> expression);

    Task<(int, List<TResult>)> GetListAllDTOs<TResult>(
        Expression<Func<T, bool>> expression,
        Expression<Func<T, TResult>> selector
    );

    Task<List<TResult>> GetDistinctValues<TResult>(
    Expression<Func<T, bool>> expression,
    Expression<Func<T, object>> groupByExpression,
    Expression<Func<IGrouping<object, T>, TResult>> selectExpression);

    // Task<List<TResult>> GetDistinctValues<TResult>(Expression<Func<T, bool>> expression, Expression<Func<T, object>> DistinctBy, Expression<Func<T, TResult>> selectExpression);

    // Complex generics
    Task<(int, List<TResult>)> GetListPaginated<TResult>(
                                            Expression<Func<T, bool>> expression,
                                            Expression<Func<T, TResult>> selector,
                                            Expression<Func<T, object>> orderBy,
                                            Expression<Func<T, object>> orderThenBy,
                                            bool sortOrder,
                                            int currentPage,
                                            int pageSize
                                        );

    Task<T> GetFirstOrDefaultInclude(
        Expression<Func<T, bool>> filter,
        params Expression<Func<T, object>>[] includes
    );

    Task<List<TResult>> GetListDesired<TResult>(Expression<Func<T, bool>> expression, Expression<Func<T, TResult>> selectExpression);
}
