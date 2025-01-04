﻿using Book.Data;
using Book.Interfaces;
using BookApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Test
{
    public class Test
    {
        private Mock<IDataRepository> _mockRepository;
        private BooksController _controller;

        [Fact]
        public async Task AddBook_ValidBook_ReturnsCreatedAtAction()
        {
            // Arrange
            var newBook = new Books { ISBN = "1234567890", Title = "Test Book", Author = "Test Author" };
            _mockRepository = new Mock<IDataRepository>();
            _controller = new BooksController(_mockRepository.Object);
            _mockRepository.Setup(repo => repo.SaveBooksAsync(newBook)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddBook(newBook);

            // Assert
            var actionResult = result as CreatedAtActionResult; // Явное приведение типа
            Assert.NotNull(actionResult); // Убедитесь, что приведение успешно
            Assert.Equal("GetBookByISBN", actionResult.ActionName);
            Assert.Equal(newBook, actionResult.Value); // Используем Assert.Equal
        }
    }
}