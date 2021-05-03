using Alura.ListaLeitura.Modelos;
using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.Api.Formatters
{
    public class LivroCsvFormatter : TextOutputFormatter
    {
        public LivroCsvFormatter()
        {
            var textCsvMIMEType = MediaTypeHeaderValue.Parse("text/csv");
            var appCsvMIMEType = MediaTypeHeaderValue.Parse("application/csv");

            SupportedMediaTypes.Add(textCsvMIMEType);
            SupportedMediaTypes.Add(appCsvMIMEType);

            SupportedEncodings.Add(Encoding.UTF8);
        }

        protected override bool CanWriteType(Type type)
        {
            return type is LivroApi;
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var livroEmCsv = "";
            
            if (context.Object is LivroApi)
            {
                var livro = context.Object as LivroApi;

                livroEmCsv = $"{livro.Titulo};{livro.Subtitulo};{livro.Autor};{livro.Lista}";
            }

            using (var writer = context.WriterFactory(context.HttpContext.Response.Body, selectedEncoding))
            {
                return writer.WriteAsync(livroEmCsv);
            } // garante writer.Close();
        }
    }
}
