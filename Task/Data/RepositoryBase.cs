using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Data;

public class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    private readonly AppDbContext _appDbContext;

    public RepositoryBase(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public IQueryable<T> GetAll(bool trackChanges)
    {
        try
        {
            if (trackChanges)
            {
                var response = _appDbContext.Set<T>();
                return response;
            }
            else
            {
                var response = _appDbContext.Set<T>().AsNoTracking();
                return response;
            }
        }
        catch (Exception error)
        {
            throw new ApplicationException(error.Message);
        }
    }

    public async Task<List<T>> GetAllAsync(bool trackChanges)
    {
        try
        {
            if (trackChanges)
            {
                var response = await _appDbContext.Set<T>().AsNoTracking().ToListAsync();
                return response;
            }
            else
            {
                var response = await _appDbContext.Set<T>().ToListAsync();
                return response;
            }
        }
        catch (Exception error)
        {
            throw new ApplicationException(error.Message);
        }
    }

    public IQueryable<T> Get(Expression<Func<T, bool>> expression, bool trackChanges)
    {
        try
        {
            if (trackChanges)
            {
                var response = _appDbContext.Set<T>().Where(expression);
                return response;
            }
            else
            {
                var response = _appDbContext.Set<T>().Where(expression).AsNoTracking();
                return response;
            }
        }
        catch (Exception error)
        {
            throw new ApplicationException(error.Message);
        }
    }

    public async Task<List<T>> GetAsync(Expression<Func<T, bool>> expression, bool trackChanges)
    {
        try
        {
            if (trackChanges)
            {
                var response = await _appDbContext.Set<T>().Where(expression).AsNoTracking().ToListAsync();
                return response;
            }
            else
            {
                var response = await _appDbContext.Set<T>().Where(expression).ToListAsync();
                return response;
            }
        }
        catch (Exception error)
        {
            throw new ApplicationException(error.Message);
        }
    }

    public void Update(T entity)
    {
        try
        {
            _appDbContext.Set<T>().Update(entity);
        }
        catch (Exception error)
        {
            throw new ApplicationException(error.Message);
        }
    }

    public void Create(T entity)
    {
        try
        {
            _appDbContext.Set<T>().Add(entity);
        }
        catch (Exception error)
        {
            throw new ApplicationException(error.Message);
        }
    }

    public void Delete(T entity)
    {
        try
        {
            _appDbContext.Set<T>().Remove(entity);
        }
        catch (Exception error)
        {
            throw new ApplicationException(error.Message);
        }
    }

    public async Task CreateAsync(T entity)
    {
        try
        {
            await _appDbContext.Set<T>().AddAsync(entity);
        }
        catch (Exception error)
        {
            throw new ApplicationException(error.Message);
        }
    }
    
    public async Task MultiCreateAsync(List<T> entities)
    {
        try
        {
            await _appDbContext.Set<T>().AddRangeAsync(entities);
        }
        catch (Exception error)
        {
            throw new ApplicationException(error.Message);
        }
    }

    public void MultiUpdate(List<T> entities)
    {
        try
        {
            _appDbContext.Set<T>().UpdateRange(entities);
        }
        catch (Exception error)
        {
            throw new ApplicationException(error.Message);
        }
    }

    public void MultiDelete(List<T> entities)
    {
        try
        {
             _appDbContext.Set<T>().RemoveRange(entities);
        }
        catch (Exception error)
        {
            throw new ApplicationException(error.Message);
        }
    }

    
    public void Save()
    {
        _appDbContext.SaveChanges();
    }
    
    public async Task SaveAsync()
    {
        await _appDbContext.SaveChangesAsync();
    }
}