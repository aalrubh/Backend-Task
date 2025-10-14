using System.Linq.Expressions;

namespace MyApp.Data;

public interface IRepositoryBase<T> where T : class
{
    public IQueryable<T> GetAll(bool trackChanges);
    public Task<List<T>> GetAllAsync(bool trackChanges);
    public IQueryable<T> Get(Expression<Func<T, bool>> expression, bool trackChanges);
    public Task<List<T>> GetAsync(Expression<Func<T, bool>> expression, bool trackChanges);
    public void Create(T entity);
    public Task CreateAsync(T entity);
    public void Update(T entity);
    public void Delete(T entity);
}