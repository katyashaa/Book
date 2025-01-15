using Book.Data;

namespace Book.Interfaces;

// Интерфейс репозитория данных для работы с книгами
public interface IDataRepository
{
    Task SaveBooksAsync(Books book);
    Task<List<Books>> SearchBooksByTitleAsync(string title);
    Task<List<Books>> SearchBooksByAuthorAsync(string author);
    Task<List<Books>> SearchBooksByISBNAsync(string isbn);
    Task<List<Books>> SearchBooksByKeywordAsync(string keyword);
    Task DeleteBookByIsbnAsync(string isbn);
}