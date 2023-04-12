using EntityLayer.Entities;
using EntityLayer.RequestFeautures;
using System.Linq.Expressions;


namespace Repositories.Abstract
{
    public interface IBookRepository : IGenericRepository<Book>
    {
        Task<PagedList<Book>> GetAllBooksWithParamsAsync(Expression<Func<Book, bool>>? filter,BookParams bookParams, bool trackingOption);
    }
}
