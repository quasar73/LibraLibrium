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

    public Book() { }

    public Book(string ownerId, int bookId)
    {
        _ownerId = ownerId;
        _bookId = bookId;
    }
}
