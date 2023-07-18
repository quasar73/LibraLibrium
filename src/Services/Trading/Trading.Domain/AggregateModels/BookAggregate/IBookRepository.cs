namespace LibraLibrium.Services.Trading.Domain.AggregateModels.BookAggregate;

public interface IBookRepository : IRepository<Book>
{
    Task<Book> GetById(int id);
}
