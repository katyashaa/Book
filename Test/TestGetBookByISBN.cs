using Book.Data;
using Book.Interfaces;
using BookApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Test;

public class TestGetBookByISBN
{
    private readonly BooksController _controller;
    private readonly Mock<IDataRepository> _mockRepository;

    public TestGetBookByISBN()
    {
        _mockRepository = new Mock<IDataRepository>();
        _controller = new BooksController(_mockRepository.Object);
    }

    [Fact]
    public async Task GetBookByISBN_ValidISBN()
    {
        var testIsbn = "3545-347654-789";
        var expectedBook = new Books { ISBN = testIsbn, Title = "Мастер и Маргарита", Author = "Михаил Булгаков" };
        _mockRepository
            .Setup(repo => repo.SearchBooksByISBNAsync(testIsbn))
            .ReturnsAsync(new List<Books> { expectedBook });

        var result = await _controller.GetBookByISBN(testIsbn);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var book = Assert.IsType<Books>(okResult.Value); // Проверка типа возвращенной книги
        Assert.Equal(expectedBook.ISBN, book.ISBN);
        Assert.Equal(expectedBook.Title, book.Title);
        Assert.Equal(expectedBook.Author, book.Author);
    }

    [Fact]
    public async Task GetBookByISBN_InvalidISBN()
    {
        var testIsbn = "0000000000"; // Несуществующий ISBN
        _mockRepository
            .Setup(repo => repo.SearchBooksByISBNAsync(testIsbn))
            .ReturnsAsync(new List<Books>()); // Пустой список

        var result = await _controller.GetBookByISBN(testIsbn);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("Книга с указанным ISBN не найдена.", notFoundResult.Value); // Проверка сообщения
    }

    [Fact]
    public async Task GetBookByISBN_BadRequest()
    {
        var testIsbn = "1234567890";
        _mockRepository
            .Setup(repo => repo.SearchBooksByISBNAsync(testIsbn))
            .ThrowsAsync(new Exception("Ошибка базы данных"));

        var result = await _controller.GetBookByISBN(testIsbn);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Contains("Ошибка при получении книги",
            badRequestResult.Value.ToString()); // Проверка сообщения об ошибке
    }
}