namespace LibraLibrium.Services.Books.API.Entities;

public class Book
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public int? NumberOfPages { get; set; }

    public int? BookSampleId { get; set; }

    public string OwnerIdentity { get; set; }

    public Book()
    {
    }

    public Book(string title, string description, int numberOfPages, string ownerIdentity)
    {
        Title = title;
        Description = description;
        NumberOfPages = numberOfPages;
        OwnerIdentity = ownerIdentity;
        BookSampleId = null;
    }

    public Book(int bookSampleId, string ownerIdentity)
    {
        Title = null;
        Description = null;
        NumberOfPages = null;
        OwnerIdentity = ownerIdentity;
        BookSampleId = bookSampleId;
    }
}
