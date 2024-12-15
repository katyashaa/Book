using Book.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Book.Data
{
    public class DatabaseConnection : IDataRepository
    {
        private readonly BookContext _context;

        public DatabaseConnection(BookContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task SaveBooksAsync(Books books)
        {
            if (books == null) throw new ArgumentNullException(nameof(books));

            try
            {
                if (!await _context.Books.AnyAsync(b => b.ISBN == books.ISBN))
                {
                    await _context.Books.AddAsync(books);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving book: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        public async Task<List<Books>> LoadBooksAsync()
        {
            return await _context.Books.ToListAsync();
        }
        
        public async Task<List<Books>> SearchBooksAsync(Func<IQueryable<Books>, IQueryable<Books>> queryBuilder)
        {
            return await queryBuilder(_context.Books).ToListAsync();
        }
                
        public async Task<List<Books>> SearchBooksByTitleAsync(string title)
        {
            return await SearchBooksAsync(query => query.Where(b => EF.Functions.ILike(b.Title, $"%{title}%")));
        }
                
        public async Task<List<Books>> SearchBooksByAuthorAsync(string author)
        {
            return await SearchBooksAsync(query => query.Where(b => EF.Functions.ILike(b.Author, $"%{author}%")));
        }
                
        public async Task<List<Books>> SearchBooksByISBNAsync(string isbn)
        {
            return await SearchBooksAsync(query => query.Where(b => b.ISBN == isbn));
        }
                
        public async Task<List<Books>> SearchBooksByKeywordAsync(string keyword)
        {
            return await SearchBooksAsync(query => query.Where(b => EF.Functions.ILike(b.Keywords, $"%{keyword}%")));
        }
        
        public async Task DeleteBookByIsbnAsync(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
                throw new ArgumentException("ISBN не может быть пустым.", nameof(isbn));

            try
            {
                var book = await _context.Books.FirstOrDefaultAsync(b => b.ISBN == isbn);

                if (book != null)
                {
                    _context.Books.Remove(book);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new KeyNotFoundException("Книга с указанным ISBN не найдена.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting book: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

    }
}
