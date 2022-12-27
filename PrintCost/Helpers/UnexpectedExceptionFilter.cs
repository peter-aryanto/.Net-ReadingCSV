using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace PrintCost.Helpers
{
  public class UnexpectedExceptionFilter : ExceptionFilterAttribute
  {
    private readonly ILogger<UnexpectedExceptionFilter> _logger;

    public UnexpectedExceptionFilter(ILogger<UnexpectedExceptionFilter> logger)
    {
      _logger = logger;
    }

    public override void OnException(ExceptionContext context)
    {
      _logger.LogError(context.Exception, context.Exception.Message);

      context.ExceptionHandled = true;
      context.HttpContext.Response.Clear();
      context.HttpContext.Response.ContentType = "application/json";
      context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
      var errorObject = new
      {
        Error = context.Exception.Message + context.Exception.StackTrace,
      };
      context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(errorObject));
    }
  }
}