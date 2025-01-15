using Book.Data;
using Book.Interfaces;
using BookApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Test;

public class TestSearchByTitle
{
    private readonly BooksController _controller;
    private readonly Mock<IDataRepository> _mockRepository;

    public TestSearchByTitle()
    {
        _mockRepository = new Mock<IDataRepository>();
        _controller = new BooksController(_mockRepository.Object);
    }

    [Fact]
    public async Task SearchBooksByTitle_ValidTitle_ReturnsBooks()
    {
        // Arrange
        var testTitle = "Test Book";
        var expectedBooks = new List<Books>
        {
            new() { ISBN = "678-98578-876", Title = "Мастер и Маргарита", Author = "Михаил Булгаков" },
            new() { ISBN = "456-876", Title = "Преступление и наказание", Author = "Фёдор Достоевский" }
        };

        _mockRepository
            .Setup(repo => repo.SearchBooksByTitleAsync(testTitle))
            .ReturnsAsync(expectedBooks);

        // Act
        var result = await _controller.SearchByTitle(testTitle);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var books = Assert.IsType<List<Books>>(okResult.Value);
        Assert.Equal(expectedBooks.Count, books.Count); // Проверка количества книг
        Assert.Equal(expectedBooks[0].Title, books[0].Title); // Проверка названия первой книги
    }

    [Fact]
    public async Task SearchBooksByTitle_ExceptionThrown_ReturnsBadRequest()
    {
        // Arrange
        var testTitle = "Error Book";
        _mockRepository
            .Setup(repo => repo.SearchBooksByTitleAsync(testTitle))
            .ThrowsAsync(new Exception("Ошибка базы данных"));

        // Act
        var result = await _controller.SearchByTitle(testTitle);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Contains("Ошибка при поиске книги", badRequestResult.Value.ToString());
    }
}