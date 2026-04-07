using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.ExceptionHandling
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case ValidationException validationException:
                    _logger.LogWarning(validationException, "Validation error occurred.");

                    context.Result = new BadRequestObjectResult(new
                    {
                        Message = "Validation failed.",
                        Errors = validationException.Errors.Select(x => x.ErrorMessage).ToList()
                    });
                    context.ExceptionHandled = true;
                    break;

                case InvalidOperationException invalidOperationException:
                    _logger.LogWarning(invalidOperationException, "Invalid operation occurred.");

                    context.Result = new BadRequestObjectResult(new
                    {
                        Message = invalidOperationException.Message
                    });
                    context.ExceptionHandled = true;
                    break;

                default:
                    _logger.LogError(context.Exception, "An unexpected error occurred.");

                    context.Result = new ObjectResult(new { Message = "An unexpected error occurred." })
                    {
                        StatusCode = StatusCodes.Status500InternalServerError
                    };

                    context.ExceptionHandled = true;
                    break;
            }
        }
    }
}