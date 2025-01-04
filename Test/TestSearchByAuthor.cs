using Book.Data;
using Book.Interfaces;
using BookApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Test
{
    public class TestSearchByAuthor
    {
        private Mock<IDataRepository> _mockRepository;
        private BooksController _controller;

        public TestSearchByAuthor()
        {
            _mockRepository = new Mock<IDataRepository>();
            _controller = new BooksController(_mockRepository.Object);
        }

        [Fact]
        public async Task SearchByAuthor_ValidAuthor()
        {
            // Arrange
            var testAuthor = "Test Author";
            var expectedBooks = new List<Books>
            {
                new Books { ISBN = "567898", Title = "Мастер и Маргарита", Author = testAuthor },
                new Books { ISBN = "567-87345-87", Title = "Преступление и наказание", Author = testAuthor }
            };

            _mockRepository
                .Setup(repo => repo.SearchBooksByAuthorAsync(testAuthor))
                .ReturnsAsync(expectedBooks);

            // Act
            var result = await _controller.SearchByAuthor(testAuthor);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result); 
            var books = Assert.IsType<List<Books>>(okResult.Value); 
            Assert.Equal(expectedBooks.Count, books.Count); 
            Assert.Equal(expectedBooks[0].Author, books[0].Author);
        }

        [Fact]
        public async Task SearchByAuthor_InvalidAuthor()
        {
            // Arrange
            var testAuthor = "Лев Толстой";
            _mockRepository
                .Setup(repo => repo.SearchBooksByAuthorAsync(testAuthor))
                .ReturnsAsync(new List<Books>()); // Пустой список

            // Act
            var result = await _controller.SearchByAuthor(testAuthor);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result); // Проверка, что возвращен NotFoundObjectResult
            Assert.Equal("Книги с таким автором не найдены.", notFoundResult.Value); // Проверка сообщения
        }

        [Fact]
        public async Task SearchByAuthor_BadRequest()
        {
            // Arrange
            var testAuthor = "Error Author";
            _mockRepository
                .Setup(repo => repo.SearchBooksByAuthorAsync(testAuthor))
                .ThrowsAsync(new Exception("Ошибка базы данных"));

            // Act
            var result = await _controller.SearchByAuthor(testAuthor);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result); // Проверка, что возвращен BadRequestObjectResult
            Assert.Contains("Ошибка при поиске книги", badRequestResult.Value.ToString()); // Проверка сообщения об ошибке
        }
    }
}
