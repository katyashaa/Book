using Book.Data;

namespace Book.Interfaces
{
    public interface IDataRepository
    {
        Task SaveBooksAsync(Books book);
        Task<List<Books>> LoadBooksAsync();
        Task<List<Books>> SearchBooksByTitleAsync(string title);
        Task<List<Books>> SearchBooksByAuthorAsync(string author);
        Task<List<Books>> SearchBooksByISBNAsync(string isbn);
        Task<List<Books>> SearchBooksByKeywordAsync(string keyword);
        Task DeleteBookByIsbnAsync(string isbn);
    }
}