﻿using Alura.ListaLeitura.Modelos;
using System.Linq;

namespace Alura.WebAPI.Api.Models
{
    public static class LivroFiltroExtensions
    {
        public static IQueryable<Livro> AplicaFiltros(this IQueryable<Livro> query, LivroFiltro filtro)
        {
            if (filtro != null)
            {
                if (!string.IsNullOrEmpty(filtro.Titulo))
                {
                    query = query.Where(l => l.Titulo.Contains(filtro.Titulo));
                }

                if (!string.IsNullOrEmpty(filtro.Subtitulo))
                {
                    query = query.Where(l => l.Subtitulo.Contains(filtro.Subtitulo));
                }
                
                if (!string.IsNullOrEmpty(filtro.Autor))
                {
                    query = query.Where(l => l.Autor.Contains(filtro.Autor));
                }
                
                if (!string.IsNullOrEmpty(filtro.Lista))
                {
                    query = query.Where(l => l.Lista == filtro.Lista.ParaTipo());
                }
            }

            return query;
        } 
    }
    public class LivroFiltro
    {
        public string Autor { get; set; }
        public string Titulo { get; set; }
        public string Subtitulo { get; set; }
        public string Lista { get; set; }
    }
}