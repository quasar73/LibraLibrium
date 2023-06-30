namespace LibraLibrium.Services.Catalog.Core.Entities;

public class Author
{
    public int Id { get; set; }

    public string FullName { get; set; }

    public string OriginalName { get; set; }

    public ICollection<BookSample> BookSamples { get; set; }
}
