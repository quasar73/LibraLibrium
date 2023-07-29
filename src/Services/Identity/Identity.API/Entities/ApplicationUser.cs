using Microsoft.AspNetCore.Identity;

namespace LibraLibrium.Services.Identity.API.Entities;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; }

    public string City { get; set; }

    public string State { get; set; }

    public string Country { get; set; }
}
