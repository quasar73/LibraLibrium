namespace LibraLibrium.Services.Identity.API.Models.AccountViewModels;

public record LoginViewModel
{
    [Required]
    public string Username { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public string ReturnUrl { get; set; }
}