using System.Collections;
using Book.Interfaces;

namespace Book.Data
{
    // Класс книги, реализующий интерфейс ISearchable
    public class Books : ISearchable, IEnumerable<string>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public string Year { get; set; }
        public string Description { get; set; }
        //public List<string> Keywords { get; set; }
        public string Keywords { get; set; }

        public Books() {}

        public Books(string title, string author, string isbn, string year, string keywords, string description)
        {
            Title = title;
            Author = author;
            ISBN = isbn;
            Year = year;
            Description = description;
            Keywords = keywords;

            
        }
        
        public override string ToString()
        {
            //string keywordFormatted = string.Join(", ", Keywords);
            return $"Title: {Title}\nAuthor: {Author}\nISBN: {ISBN}\nYear: {Year}\nKeywords: {Keywords}\nDescription: {Description}\n";
        }
        // Реализация интерфейса IEnumerable<string>
        public IEnumerator<string> GetEnumerator()
        {
            // Если Keywords не пустая, то делим строку по запятой
            if (!string.IsNullOrEmpty(Keywords))
            {
                foreach (var keyword in Keywords.Split(','))
                {
                    yield return keyword.Trim(); // Возвращаем ключевое слово, убрав лишние пробелы
                }
            }
        }

        // Реализация интерфейса IEnumerable
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
