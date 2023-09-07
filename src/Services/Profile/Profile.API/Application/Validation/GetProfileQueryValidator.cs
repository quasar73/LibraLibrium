using FluentValidation;
using LibraLibrium.Services.Profile.API.Application.Queries;

namespace LibraLibrium.Services.Profile.API.Application.Validation;

public class GetProfileQueryValidator : AbstractValidator<GetProfileQuery>
{
    public GetProfileQueryValidator(ILogger<GetProfileQueryValidator> logger)
    {
        RuleFor(x => x.Identity)
            .NotEmpty()
            .Must((identity) => Guid.TryParse(identity, out Guid guid) && Guid.Empty.ToString() != identity);

        logger.LogTrace("INSTANCE CREATED - {ClassName}", GetType().Name);
    }
}
