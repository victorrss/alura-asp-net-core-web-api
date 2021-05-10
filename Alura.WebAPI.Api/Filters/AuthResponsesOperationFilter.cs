﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace Alura.WebAPI.Api.Filters
{
    public class AuthResponsesOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var authAttributes = context.MethodInfo
             .DeclaringType
             .GetCustomAttributes(true)
             .Union(context.MethodInfo.GetCustomAttributes(true))
             .OfType<AuthorizeAttribute>();

            if (authAttributes.Any())
                operation.Responses.Add(
                    "401",
                    new OpenApiResponse { Description = "Unauthorized" });
        }
    }
}
