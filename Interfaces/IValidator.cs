using Book.Data;

namespace Book.Interfaces;

// Интерфейс для валидации книг
public interface IValidator
{
    Task ValidateBookAsync(Books book, IDataRepository repository);
}