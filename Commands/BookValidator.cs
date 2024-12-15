using Book.Interfaces;
using Book.Data;

namespace Book.Commands
{
    public class BookValidator : IValidator
    {
        // Метод для валидации книги
        public async Task ValidateBookAsync(Books book, IDataRepository repository)
        {
            if (book == null)
            {
                throw new ArgumentNullException(nameof(book), "Ошибка: Книга не может быть null.");
            }

            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository), "Ошибка: Каталог не может быть null.");
            }
            
            // Проверка на заполнение всех обязательных полей книги
            if (string.IsNullOrWhiteSpace(book.Title) || string.IsNullOrWhiteSpace(book.Author) ||
                string.IsNullOrWhiteSpace(book.ISBN) ||
                string.IsNullOrWhiteSpace(book.Description))
            {
                throw new InvalidException("Ошибка: Все поля должны быть заполнены.");
            }
            
            // Поиск книг в репозитории по заданному ISBN
            var existingBooks = await repository.SearchBooksByISBNAsync(book.ISBN);
            if (existingBooks.Any(b => b.ISBN.Equals(book.ISBN, StringComparison.OrdinalIgnoreCase)))
            {
                throw new DuplicateBookException("Ошибка: Книга с таким ISBN уже существует в базе данных.");
            }
        }
    }
}