using EntityLayer.RequestFeautures;
using Microsoft.EntityFrameworkCore;
using Repositories.Abstract;
using System.Linq.Expressions;


namespace Repositories.Concrate
{
    public abstract class GenericRepository<Tentity> : IGenericRepository<Tentity> where Tentity : class, new()
    {
        private readonly RepositoryContext _context;
        public GenericRepository(RepositoryContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Tentity entity)
        {
            await _context.Set<Tentity>().AddAsync(entity);
        }
        public void Update(Tentity entity)
        {
            _context.Set<Tentity>().Update(entity);
        }
        public void Delete(Tentity entity)
        {
            _context.Set<Tentity>().Remove(entity);
        }

        public IQueryable<Tentity> GetAllAsync(Expression<Func<Tentity, bool>>? filter, bool trackingOption)
        {
            var result = _context.Set<Tentity>().AsQueryable();
            if (filter != null)
                result=result.Where(filter).AsQueryable();
            if (!trackingOption)
                result=result.AsNoTracking();
            return result.AsQueryable();
        }

        public async Task<Tentity> GetOneAsync(Expression<Func<Tentity, bool>> filter, bool trackingOption)
        {
            var result = _context.Set<Tentity>();
            if (!trackingOption)
                result.AsNoTracking();
            return await result.FirstOrDefaultAsync(filter);
        }
    }
}
