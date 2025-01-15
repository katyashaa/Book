using Book.Data;
using Book.Interfaces;
using BookApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Test;

public class TestAddBook
{
    private BooksController _controller;
    private Mock<IDataRepository> _mockRepository;

    [Fact]
    public async Task AddBook()
    {
        var newBook = new Books { ISBN = "234-654-345", Title = "Мастер и Маргарита", Author = "Михаил Булгаков" };
        _mockRepository = new Mock<IDataRepository>();
        _controller = new BooksController(_mockRepository.Object);
        _mockRepository.Setup(repo => repo.SaveBooksAsync(newBook)).Returns(Task.CompletedTask);

        var result = await _controller.AddBook(newBook);

        var actionResult = result as CreatedAtActionResult;
        Assert.NotNull(actionResult);
        Assert.Equal("GetBookByISBN", actionResult.ActionName);
        Assert.Equal(newBook, actionResult.Value);
    }
}