namespace Book.Interfaces;

// Интерфейс для объектов, которые можно искать
public interface ISearchable
{
    string Keywords { get; }
    string Title { get; }
    string Author { get; }
    string ISBN { get; }
    string Year { get; }
    string Description { get; }
}