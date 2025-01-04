using Book.Data;
using Book.Interfaces;
using BookApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Test
{
    public class TestGetBookByISBN
    {
        private Mock<IDataRepository> _mockRepository;
        private BooksController _controller;

        public TestGetBookByISBN()
        {
            _mockRepository = new Mock<IDataRepository>();
            _controller = new BooksController(_mockRepository.Object);
        }

        [Fact]
        public async Task GetBookByISBN_ValidISBN()
        {
            // Arrange
            var testIsbn = "3545-347654-789";
            var expectedBook = new Books { ISBN = testIsbn, Title = "Test Book", Author = "Test Author" };
            _mockRepository
                .Setup(repo => repo.SearchBooksByISBNAsync(testIsbn))
                .ReturnsAsync(new List<Books> { expectedBook });

            // Act
            var result = await _controller.GetBookByISBN(testIsbn);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result); 
            var book = Assert.IsType<Books>(okResult.Value); // Проверка типа возвращенной книги
            Assert.Equal(expectedBook.ISBN, book.ISBN); 
            Assert.Equal(expectedBook.Title, book.Title); 
            Assert.Equal(expectedBook.Author, book.Author);
        }

        [Fact]
        public async Task GetBookByISBN_InvalidISBN()
        {
            // Arrange
            var testIsbn = "0000000000"; // Несуществующий ISBN
            _mockRepository
                .Setup(repo => repo.SearchBooksByISBNAsync(testIsbn))
                .ReturnsAsync(new List<Books>()); // Пустой список

            // Act
            var result = await _controller.GetBookByISBN(testIsbn);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result); // Проверка, что возвращен NotFoundObjectResult
            Assert.Equal("Книга с указанным ISBN не найдена.", notFoundResult.Value); // Проверка сообщения
        }
        
        [Fact]
        public async Task GetBookByISBN_BadRequest()
        {
            // Arrange
            var testIsbn = "1234567890";
            _mockRepository
                .Setup(repo => repo.SearchBooksByISBNAsync(testIsbn))
                .ThrowsAsync(new Exception("Ошибка базы данных"));

            // Act
            var result = await _controller.GetBookByISBN(testIsbn);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result); // Проверка, что возвращен BadRequestObjectResult
            Assert.Contains("Ошибка при получении книги", badRequestResult.Value.ToString()); // Проверка сообщения об ошибке
        }
    }
}
