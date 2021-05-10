﻿using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alura.WebAPI.Api.Filters
{
    public class AddInfoToParamVersionOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var paramVersion = operation.Parameters
               .Where(p => p.Name == "version")
               .FirstOrDefault();
            if (paramVersion != null)
            {
                paramVersion.Description = "Versão da API";
            }
        }
    }
}
