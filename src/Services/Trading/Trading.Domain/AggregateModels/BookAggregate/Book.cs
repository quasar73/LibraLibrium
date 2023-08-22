namespace LibraLibrium.Services.Trading.Domain.AggregateModels.BookAggregate;

public class Book
    : Entity, IAggregateRoot
{
    public string OwnerId => _ownerId;
    private string _ownerId;

    private int _bookId;
    public virtual int BookId
    {
        get
        {
            return _bookId;
        }
        protected set
        {
            _bookId = value;
        }
    }

    private Book() 
    {
        _ownerId = string.Empty;
        _bookId = 0;
    }

    public Book(string ownerId, int bookId)
    {
        if (!Guid.TryParse(ownerId, out Guid result))
        {
            throw new ArgumentException("Owner Id must be the GUID.");
        }

        _ownerId = ownerId;
        _bookId = bookId;
    }
}
