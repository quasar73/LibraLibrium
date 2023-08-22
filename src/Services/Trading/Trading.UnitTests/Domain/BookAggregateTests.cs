namespace LibraLibrium.Services.Trading.UnitTests.Domain;

public class BookAggregateTests
{
    public BookAggregateTests() { }

    [Fact]
    public void Create_book_success()
    {
        //Arrange
        var ownerId = Guid.NewGuid().ToString();
        var bookId = 1;

        //Act 
        var fakeBook = new Book(ownerId, bookId);

        //Assert
        Assert.NotNull(fakeBook);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("identity")]
    public void Invalid_owner_id(string ownerId)
    {
        //Arrange
        var bookId = 1;

        //Act - Assert
        Assert.Throws<ArgumentException>(() => new Book(ownerId, bookId));
    }
}
