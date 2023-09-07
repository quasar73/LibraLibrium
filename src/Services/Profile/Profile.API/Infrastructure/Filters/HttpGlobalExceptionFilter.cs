using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using LibraLibrium.Services.Profile.Domain.Exceptions;
using LibraLibrium.Services.Profile.API.Infrastructure.ActionResult;
using LibraLibrium.Services.Profile.Infrastructure.Exceptions;

namespace LibraLibrium.Services.Profile.API.Infrastructure.Filters;

public class HttpGlobalExceptionFilter : IExceptionFilter
{
    private readonly IWebHostEnvironment env;
    private readonly ILogger<HttpGlobalExceptionFilter> logger;

    public HttpGlobalExceptionFilter(IWebHostEnvironment env, ILogger<HttpGlobalExceptionFilter> logger)
    {
        this.env = env;
        this.logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        logger.LogError(new EventId(context.Exception.HResult),
            context.Exception,
            context.Exception.Message);

        if (context.Exception.GetType() == typeof(ProfileDomainException))
        {
            var problemDetails = new ValidationProblemDetails()
            {
                Instance = context.HttpContext.Request.Path,
                Status = StatusCodes.Status400BadRequest,
                Detail = "Please refer to the errors property for additional details."
            };

            problemDetails.Errors.Add("DomainValidations", new string[] { context.Exception.Message.ToString() });

            context.Result = new BadRequestObjectResult(problemDetails);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
        else if (context.Exception.GetType() == typeof(NotFoundException))
        {
            var problemDetails = new ProblemDetails()
            {
                Instance = context.HttpContext.Request.Path,
                Status = StatusCodes.Status404NotFound,
                Detail = "Please refer to the errors property for additional details."
            };

            problemDetails.Detail = context.Exception.Message.ToString();

            context.Result = new NotFoundObjectResult(problemDetails);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
        }
        else
        {
            var json = new JsonErrorResponse
            {
                Messages = new[] { "An error occur. Try it again." }
            };

            if (env.IsDevelopment())
            {
                json.DeveloperMessage = context.Exception;
            }

            context.Result = new InternalServerErrorObjectResult(json);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
        context.ExceptionHandled = true;
    }

    private class JsonErrorResponse
    {
        public string[] Messages { get; set; } = null!;

        public object DeveloperMessage { get; set; } = null!;
    }
}