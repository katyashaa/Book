using Book.Data;
using Book.Interfaces;
using BookApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Test;

public class BooksControllerTests
{
    private readonly Mock<IDataRepository> _repositoryMock;
    private readonly BooksController _controller;

    public BooksControllerTests()
    {
        _repositoryMock = new Mock<IDataRepository>();
        _controller = new BooksController(_repositoryMock.Object);
    }

    [Fact]
    public async Task TestAddBook()
    {
        var newBook = new Books
        {
            Title = "Мастер и Маргарита",
            Author = "Михаил Булгаков",
            ISBN = "978-5-17-069829-5",
            Year = "1967",
            Description = "Мистический роман о любви, дьяволе и Москве",
            Keywords = "дьявол, любовь"
        };

        _repositoryMock.Setup(repo => repo.SaveBooksAsync(newBook))
            .Returns(Task.CompletedTask);
        
        var result = await _controller.AddBook(newBook);
        
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetBookByISBN), createdResult.ActionName);
        Assert.Equal(newBook.ISBN, ((Books)createdResult.Value).ISBN);
    }

    [Fact]
    public async Task GetBookByISBN_BookExists()
    {
        var isbn = "12345";
        var expectedBook = new Books
        {
            Title = "Мастер и Маргарита",
            Author = "Михаил Булгаков",
            ISBN = "978-5-17-069829-5",
            Year = "1967",
            Description = "Мистический роман о любви, дьяволе и Москве",
            Keywords = "дьявол, любовь"
        };

        _repositoryMock.Setup(repo => repo.SearchBooksByISBNAsync(isbn))
            .ReturnsAsync(new List<Books> { expectedBook });
        
        var result = await _controller.GetBookByISBN(isbn);
        
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var book = Assert.IsType<Books>(okResult.Value);
        Assert.Equal(isbn, book.ISBN);
    }

    [Fact]
    public async Task GetBookByISBN_BookDoesNotExist()
    {
        var isbn = "12345";

        _repositoryMock.Setup(repo => repo.SearchBooksByISBNAsync(isbn))
            .ReturnsAsync(new List<Books>());
        
        var result = await _controller.GetBookByISBN(isbn);
        
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("Книга с указанным ISBN не найдена.", notFoundResult.Value);
    }

    [Fact]
    public async Task DeleteBook_ShouldReturnNoContent_WhenBookExists()
    {
        var isbn = "12345";

        _repositoryMock.Setup(repo => repo.DeleteBookByIsbnAsync(isbn))
            .Returns(Task.CompletedTask);
        
        var result = await _controller.DeleteBook(isbn);
        
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteBook_DoesNotExist()
    {
        var isbn = "12345";

        _repositoryMock.Setup(repo => repo.DeleteBookByIsbnAsync(isbn))
            .ThrowsAsync(new KeyNotFoundException($"Книга с ISBN {isbn} не найдена."));
        
        var result = await _controller.DeleteBook(isbn);
        
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal($"Книга с ISBN {isbn} не найдена.", notFoundResult.Value);
    }

    [Fact]
    public void Books_ShouldSplitKeywordsIntoList()
    {
        var book = new Books
        {
            Keywords = "test, example, book"
        };

        var expectedKeywords = new List<string> { "test", "example", "book" };
        
        var keywords = book.Keywords.Split(',').Select(k => k.Trim()).ToList();
        
        Assert.Equal(expectedKeywords, keywords);
    }
}