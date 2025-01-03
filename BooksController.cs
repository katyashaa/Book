using Book.Data;
using Book.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BooksController : ControllerBase
    {
        private readonly IDataRepository _repository;

        public BooksController(IDataRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        // Добавить книгу
        [HttpPost]
        public async Task<ActionResult> AddBook([FromBody] Books newBook)
        {
            if (newBook == null)
                return BadRequest("Книга не может быть пустой.");

            try
            {
                await _repository.SaveBooksAsync(newBook);
                return CreatedAtAction(nameof(GetBookByISBN), new { isbn = newBook.ISBN }, newBook);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при добавлении книги: {ex.Message}");
            }
        }

        
      //  https://www.ozon.ru/product/electrolux-vstraivaemaya-posudomoechnaya-mashina-eem48300l-serebristyy-1594336264/?campaignId=514
        
        // Получить книгу по ISBN
        [HttpGet("{isbn}")]
        public async Task<ActionResult<Books>> GetBookByISBN([FromRoute]string isbn)
        {
            try
            {
                var book = await _repository.SearchBooksByISBNAsync(isbn);

                if (book == null || !book.Any())
                    return NotFound("Книга с указанным ISBN не найдена.");

                return Ok(book.First());
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при получении книги: {ex.Message}");
            }
        }

        // Найти книги по названию
        [HttpGet("search/title/{title}")]
        public async Task<ActionResult<IEnumerable<Books>>> SearchByTitle(string title)
        {
            try
            {
                var results = await _repository.SearchBooksByTitleAsync(title);
                return results.Any() ? Ok(results) : NotFound("Книги с таким названием не найдены.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при поиске книги: {ex.Message}");
            }
        }

        // Найти книги по автору
        [HttpGet("search/author/{author}")]
        public async Task<ActionResult<IEnumerable<Books>>> SearchByAuthor(string author)
        {
            try
            {
                var results = await _repository.SearchBooksByAuthorAsync(author);
                return results.Any() ? Ok(results) : NotFound("Книги с таким автором не найдены.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при поиске книги: {ex.Message}");
            }
        }

        // Удалить книгу по ISBN
        [HttpDelete("{isbn}")]
        public async Task<ActionResult> DeleteBook(string isbn)
        {
            try
            {
                await _repository.DeleteBookByIsbnAsync(isbn);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при удалении книги: {ex.Message}");
            }
        }
    }
}

