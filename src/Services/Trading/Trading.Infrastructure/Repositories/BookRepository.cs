namespace LibraLibrium.Services.Trading.Infrastructure.Repositories;

public class BookRepository : IBookRepository
{
    private readonly TradingContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public BookRepository(TradingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Book> GetById(int id)
    {
        var book = await _context.Books
            .Where(b => b.BookId == id)
            .SingleOrDefaultAsync();

        return book;
    }
}
