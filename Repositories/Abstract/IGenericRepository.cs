using EntityLayer.RequestFeautures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Abstract
{
    public interface IGenericRepository<TEntity> where TEntity : class, new()
    {
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        IQueryable<TEntity> GetAllAsync(Expression<Func<TEntity, bool>>? filter, bool trackingOption);
        Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> filter, bool trackingOption);
    }
}
