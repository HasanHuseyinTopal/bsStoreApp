using EntityLayer.Entities;
using EntityLayer.RequestFeautures;
using Microsoft.EntityFrameworkCore;
using Repositories.Abstract;
using System.Linq.Expressions;

namespace Repositories.Concrate
{
    public class BookRepository : GenericRepository<Book>, IBookRepository
    {
        RepositoryContext _context;
        public BookRepository(RepositoryContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PagedList<Book>> GetAllBooksWithParamsAsync(Expression<Func<Book, bool>>? filter, BookParams bookParams, bool trackingOption)
        {
            var result = _context.Set<Book>().OrderBy(prp => prp.BookId).AsQueryable();
            if (filter != null)
                result = result.Where(filter).AsQueryable();
            if (!trackingOption)
                result = result.AsNoTracking();
            return PagedList<Book>.ToPagedList(await result.ToListAsync(), bookParams.PageSize, bookParams.PageNumber);
        }
    }
}
