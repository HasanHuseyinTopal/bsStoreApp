using AutoMapper;
using EntityLayer.Entities;
using EntityLayer.Entities.DTOs;
using EntityLayer.Exceptions;
using EntityLayer.RequestFeautures;
using Microsoft.EntityFrameworkCore;
using Repositories.Abstract;
using Services.Abstract;
using System.Linq.Expressions;

namespace Services.Concrate
{

    public class BookService : IBookService
    {
        IRepositoryManager _repositoryManager;
        IMapper _mapper;
        public BookService(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }
        public async Task<Book> AddOneBookAsync(BookDTO bookModel)
        {
            if (bookModel is null)
                throw new ArgumentException(nameof(bookModel));
            var book = _mapper.Map<Book>(bookModel);
            await _repositoryManager.Book.AddAsync(book);
            await _repositoryManager.Save();
            return book;
        }
        public async Task<Book> UpdateOneBookAsync(int id, BookDTO bookModel)
        {
            var book = await İsBookNull(id, false);
            book = _mapper.Map<Book>(bookModel);
            _repositoryManager.Book.Update(book);
            await _repositoryManager.Save();
            return book;
        }
        public async Task DeleteOneBookAsync(int id)
        {
            _repositoryManager.Book.Delete(await İsBookNull(id, false));
            await _repositoryManager.Save();
        }

        public async Task<(IEnumerable<BookDTO> books, MetaData metaData)> GetAllBooksWithParamsAsync(BookParams bookParams, bool trackingOption)
        {
            if (!bookParams.ValidPriceRange)
                throw new PriceOutRangeBadRequestException();

            var booksWithData = await _repositoryManager.Book.GetAllBooksWithParamsAsync(prp => prp.BookPrice >= bookParams.MinPrice && prp.BookPrice <= bookParams.MaxPrice && (bookParams.SearchTerm != null ? prp.BookName.ToLower().Contains(bookParams.SearchTerm.ToLower()) : true), bookParams, trackingOption);

            var books = _mapper.Map<IEnumerable<BookDTO>>(booksWithData);
            return (books, booksWithData.MetaData);
        }

        public async Task<Book> GetOneBookAsync(int id, bool trackingOption = false)
        {
            return await İsBookNull(id, trackingOption);
        }


        public IQueryable<BookDTO> GetAllBooksAsync(Expression<Func<Book, bool>> filter, bool trackingOption)
        {
            var books = _repositoryManager.Book.GetAllAsync(filter, trackingOption);
            return _mapper.Map<IQueryable<BookDTO>>(books);
        }
        public async Task<Book> İsBookNull(int id, bool trackingOption)
        {
            var book = await _repositoryManager.Book.GetOneAsync(prop => prop.BookId.Equals(id), trackingOption);
            if (book is null)
                throw new BookNotFoundException(id);
            return book;
        }
    }
}
