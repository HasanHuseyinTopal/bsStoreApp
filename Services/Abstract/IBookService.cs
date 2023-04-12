using EntityLayer.Entities;
using EntityLayer.Entities.DTOs;
using EntityLayer.RequestFeautures;
using System.Linq.Expressions;

namespace Services.Abstract
{
    public interface IBookService
    {
        IQueryable<BookDTO> GetAllBooksAsync(Expression<Func<Book, bool>> filter, bool trackingOption);
        Task<(IEnumerable<BookDTO> books,MetaData metaData)> GetAllBooksWithParamsAsync(BookParams bookParams, bool trackingOption);
        Task<Book> GetOneBookAsync(int id, bool trackingOption);
        Task<Book> AddOneBookAsync(BookDTO bookModel);
        Task<Book> UpdateOneBookAsync(int id, BookDTO bookModel);
        Task DeleteOneBookAsync(int id);

    }
}
