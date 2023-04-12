using EntityLayer.Entities.DTOs;
using EntityLayer.RequestFeautures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Presentation.ActionFilters;
using Services.Abstract;
using System.Text.Json;


namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/books")]
    //[ResponseCache(CacheProfileName = "5mins")]
    [EnableRateLimiting("Fixed")]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly IServiceManager _services;
        public BooksController(IServiceManager services)
        {
            _services = services;
        }
        [HttpGet]
        //[ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetAllBooks([FromQuery] BookParams bookParams)
        {
            var pagedBooks = await _services.BookService.GetAllBooksWithParamsAsync(bookParams, false);
            HttpContext.Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagedBooks.metaData));
            var res = pagedBooks.books;
            return Ok(pagedBooks.books);
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOneBook([FromRoute(Name = "id")] int id)
        {
            var book = await _services.BookService.GetOneBookAsync(id, false);
            return Ok(book);
        }
        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> PostOneBook([FromBody] BookDTO bookModel)
        {
            var book = await _services.BookService.AddOneBookAsync(bookModel);
            return StatusCode(200, book);
        }
        [HttpDelete("{id:int}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> DeleteBook([FromRoute(Name = "id")] int id)
        {
            await _services.BookService.DeleteOneBookAsync(id);
            return NoContent();
        }
        [HttpPut("{id:int}")]
        [ServiceFilter(typeof(LogDetailsAttribute))]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> PutOneBook([FromBody] BookDTO bookModel, [FromRoute(Name = "id")] int id)
        {
            var book = await _services.BookService.UpdateOneBookAsync(id, bookModel);
            return Ok(book);
        }
        [HttpOptions]
        public IActionResult HttpOptions()
        {
            HttpContext.Response.Cookies.Append("Allow", "GET, POST, PUT, DELETE, HEAD, OPTİONS");
            return Ok();
        }
    }
}
