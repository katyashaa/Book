using Book.Commands;
using Book.Data;
using Book.Interfaces;

namespace Book.Menu
{
    class Program
    {
        static async Task Main()
        {
            IValidator validator = new BookValidator();
            BookContext context = BookContext.Init();
            IDataRepository repository = new DatabaseConnection(context);
            BookManager bookManager = new BookManager(validator, repository);
            
            while (true) // Бесконечный цикл для главного меню
            {
                Console.Clear();
                Console.WriteLine("Главное меню:");
                Console.WriteLine("1. Добавить книгу");
                Console.WriteLine("2. Найти книгу");
                Console.WriteLine("3. Удалить книгу");
                Console.WriteLine("4. Выход");
                Console.Write("Выберите пункт: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": // Если выбрано добавление книги
                        await bookManager.AddBookAsync();
                        break;
                    case "2": // Если выбрано поиск книги
                        await bookManager.FindBookAsync();
                        break;
                    case "3": // Если выбрано удалить книгу
                        await bookManager.DeleteBookAsync();
                        break;
                    case "4": // Если выбрано выход
                        return;
                    default: // Если выбор невалиден
                        Console.WriteLine("Неверный выбор. Попробуйте еще раз.");
                        break;
                }

                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.ReadKey(); // Ожидание ввода клавиши
            }
        }
    }
}