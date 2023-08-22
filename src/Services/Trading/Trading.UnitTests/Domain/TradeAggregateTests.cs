namespace LibraLibrium.Services.Trading.UnitTests.Domain;

public class TradeAggregateTests
{
    public TradeAggregateTests() { }

    [Fact]
    public void Create_trade_success()
    {
        //Arrange    
        var createdAt = new DateTime(2023, 08, 21);
        var receiverId = Guid.NewGuid().ToString();
        var senderId = Guid.NewGuid().ToString();

        //Act 
        var fakeTrade = new Trade(createdAt, receiverId, senderId);

        //Assert
        Assert.NotNull(fakeTrade);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("identity")]
    public void Invalid_receiver_id(string receiverId)
    {
        //Arrange    
        var createdAt = new DateTime(2023, 08, 21);
        var senderId = Guid.NewGuid().ToString();

        //Act - Assert
        Assert.Throws<ArgumentException>(() => new Trade(createdAt, receiverId, senderId));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("identity")]
    public void Invalid_sender_id(string senderId)
    {
        //Arrange    
        var createdAt = new DateTime(2023, 08, 21);
        var receiverId = Guid.NewGuid().ToString();

        //Act - Assert
        Assert.Throws<ArgumentException>(() => new Trade(createdAt, receiverId, senderId));
    }

    [Fact]
    public void Add_new_trade_entry()
    {
        //Arrange    
        var createdAt = new DateTime(2023, 08, 21);
        var receiverId = Guid.NewGuid().ToString();
        var senderId = Guid.NewGuid().ToString();
        var bookId = 1;
        var fakeTrade = new Trade(createdAt, receiverId, senderId);
        var expectedCount = 1;

        //Act 
        fakeTrade.AddTradeEntry(bookId, 0, senderId, createdAt, EntryType.Added);

        //Assert
        Assert.Equal(fakeTrade.Entries.Count, expectedCount);
    }

    [Fact]
    public void Invalid_trader_id_when_add_new_entry()
    {
        //Arrange    
        var createdAt = new DateTime(2023, 08, 21);
        var receiverId = Guid.NewGuid().ToString();
        var senderId = Guid.NewGuid().ToString();
        var traderId = Guid.NewGuid().ToString();
        var bookId = 1;
        var fakeTrade = new Trade(createdAt, receiverId, senderId);

        //Act - Assert
        Assert.Throws<TradingDomainException>(() => fakeTrade.AddTradeEntry(bookId, 0, traderId, createdAt, EntryType.Added));
    }

    [Fact]
    public void Invalid_generation_when_add_new_entry()
    {
        //Arrange    
        var createdAt = new DateTime(2023, 08, 21);
        var receiverId = Guid.NewGuid().ToString();
        var senderId = Guid.NewGuid().ToString();
        var bookId = 1;
        var fakeTrade = new TradeBuilder(createdAt, receiverId, senderId)
            .AddEntry(bookId, 1, senderId, createdAt, EntryType.Added)
            .Build();

        //Act - Assert
        Assert.Throws<TradingDomainException>(() => fakeTrade.AddTradeEntry(bookId, 0, senderId, createdAt, EntryType.Added));
    }

    [Fact]
    public void Close_generation()
    {
        //Arrange    
        var createdAt = new DateTime(2023, 08, 21);
        var receiverId = Guid.NewGuid().ToString();
        var senderId = Guid.NewGuid().ToString();
        var bookId = 1;
        var fakeTrade = new TradeBuilder(createdAt, receiverId, senderId)
            .AddEntry(bookId, 0, senderId, createdAt, EntryType.Added)
            .Build();

        //Act 
        fakeTrade.CloseGeneration(senderId);

        //Assert
        Assert.True(fakeTrade.GenerationClosed);
    }

    [Fact]
    public void Invalid_trader_id_when_close_generation()
    {
        //Arrange    
        var createdAt = new DateTime(2023, 08, 21);
        var receiverId = Guid.NewGuid().ToString();
        var senderId = Guid.NewGuid().ToString();
        var traderId = Guid.NewGuid().ToString();
        var bookId = 1;
        var fakeTrade = new TradeBuilder(createdAt, receiverId, senderId)
            .AddEntry(bookId, 0, senderId, createdAt, EntryType.Added)
            .Build();

        //Act - Assert
        Assert.Throws<TradingDomainException>(() => fakeTrade.CloseGeneration(traderId));
    }

    [Fact]
    public void Invalid_trader_id_in_generation_when_close_generation()
    {
        //Arrange    
        var createdAt = new DateTime(2023, 08, 21);
        var receiverId = Guid.NewGuid().ToString();
        var senderId = Guid.NewGuid().ToString();
        var bookId = 1;
        var fakeTrade = new TradeBuilder(createdAt, receiverId, senderId)
             .AddEntry(bookId, 0, senderId, createdAt, EntryType.Added)
             .Build();

        //Act - Assert
        Assert.Throws<TradingDomainException>(() => fakeTrade.CloseGeneration(receiverId));
    }
}
