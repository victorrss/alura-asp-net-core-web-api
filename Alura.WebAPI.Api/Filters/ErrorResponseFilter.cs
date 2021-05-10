using Alura.WebAPI.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alura.WebAPI.Api.Filters
{
    public class ErrorResponseFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var errResp = ErrorResponse.From(context.Exception);
            context.Result = new ObjectResult(errResp) { StatusCode = 500 };
        }
    }
}
