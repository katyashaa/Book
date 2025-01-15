using Book.Data;
using Book.Interfaces;

namespace Book.Commands;

// Класс для управления операциями с книгами
public class BookManager
{
    private readonly IDataRepository _repository;
    private readonly IValidator _validator;

    // Конструктор для инициализации зависимостей
    public BookManager(IValidator validator, IDataRepository repository)
    {
        _validator = validator;
        _repository = repository;
    }

    public async Task AddBookAsync()
    {
        try
        {
            // Запрашиваем у пользователя данные о книге
            Console.Write("Введите название книги: ");
            var title = Console.ReadLine();

            Console.Write("Введите имя автора: ");
            var author = Console.ReadLine();

            string isbn;
            while (true)
            {
                Console.Write("Введите ISBN книги (например, ISBN-10: 2-266-11156-6 или ISBN-13: 978-2-266-11156-0): ");
                isbn = Console.ReadLine();

                // Проверяем, что ISBN содержит только цифры и тире
                if (isbn.All(c => char.IsDigit(c) || c == '-')) break;

                Console.WriteLine("Ошибка: ISBN должен содержать только цифры и тире. Попробуйте еще раз.");
            }

            Console.Write("Введите год написания книги: ");
            var year = Console.ReadLine();

            Console.Write("Введите ключевые слова (через запятую): ");
            var keywords = Console.ReadLine();

            Console.Write("Введите аннотацию книги: ");
            var description = Console.ReadLine();

            // Создаем объект книги
            var newBook = new Books(title, author, isbn, year, keywords, description);

            // Выполняем валидацию книги
            await _validator.ValidateBookAsync(newBook, _repository);

            await _repository.SaveBooksAsync(newBook);
            Console.WriteLine("Книга добавлена успешно.");
        }

        catch (InvalidDataException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (DuplicateBookException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    // Метод для поиска книги
    public async Task FindBookAsync()
    {
        Console.WriteLine("Поиск книги по:");
        Console.WriteLine("1. Названию");
        Console.WriteLine("2. Имени автора");
        Console.WriteLine("3. ISBN");
        Console.WriteLine("4. Ключевым словам");
        Console.Write("Выберите пункт: ");
        var choice = Console.ReadLine();

        List<Books> results = null;

        Console.Write("Введите запрос: ");
        var query = Console.ReadLine();

        try
        {
            // Выполняем поиск в зависимости от выбранного пункта
            switch (choice)
            {
                case "1":
                    results = await _repository.SearchBooksByTitleAsync(query);
                    break;
                case "2":
                    results = await _repository.SearchBooksByAuthorAsync(query);
                    break;
                case "3":
                    results = await _repository.SearchBooksByISBNAsync(query);
                    break;
                case "4":
                    results = await _repository.SearchBooksByKeywordAsync(query);
                    break;
                default:
                    Console.WriteLine("Неверный выбор.");
                    return;
            }

            // Вывод результатов поиска
            if (results != null && results.Any())
            {
                Console.WriteLine("Результаты поиска:");
                foreach (var book in results) Console.WriteLine(book.ToString());
            }
            else
            {
                Console.WriteLine("Книги не найдены.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    // Метод для удаления книги
    public async Task DeleteBookAsync()
    {
        try
        {
            Console.Write("Введите ISBN книги, которую нужно удалить: ");
            var isbn = Console.ReadLine();

            // Проверяем, существует ли книга с таким ISBN
            var book = await _repository.SearchBooksByISBNAsync(isbn);

            if (book == null || !book.Any())
            {
                Console.WriteLine("Книга с указанным ISBN не найдена.");
                return;
            }

            // Удаляем книгу
            await _repository.DeleteBookByIsbnAsync(isbn);
            Console.WriteLine("Книга успешно удалена.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}