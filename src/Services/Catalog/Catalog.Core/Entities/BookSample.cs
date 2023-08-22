namespace LibraLibrium.Services.Catalog.Core.Entities;

public class BookSample
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public int NumberOfPages { get; set; }

    public ICollection<Author> Authors { get; set; }

    public int PublisherId { get; set; }

    public Publisher Publisher { get; set; }
}
