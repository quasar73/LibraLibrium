﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraLibrium.Services.Profile.API.Infrastructure.ActionResult;

public class InternalServerErrorObjectResult : ObjectResult
{
    public InternalServerErrorObjectResult(object error) : base(error)
    {
        StatusCode = StatusCodes.Status500InternalServerError;
    }
}
