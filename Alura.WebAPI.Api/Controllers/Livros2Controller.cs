using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Persistencia;
using Alura.WebAPI.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Alura.ListaLeitura.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/livros")]
    public class Livros2Controller : ControllerBase
    {
        private readonly IRepository<Livro> _repo;

        public Livros2Controller(IRepository<Livro> repository)
        {
            _repo = repository;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Recupera todos os livros",
            Tags = new[] { "Livros" }
        )]
        [ProducesResponseType(statusCode: 200, Type = typeof(LivroPaginado))]
        [ProducesResponseType(statusCode: 500, Type = typeof(ErrorResponse))]
        public IActionResult Todos(
            [FromQuery] LivroFiltro filtro,
            [FromQuery] LivroOrdem ordem,
            [FromQuery] LivroPaginacao paginacao
            )
        {
            var livroPaginado = _repo.All
                .AplicaFiltros(filtro)
                .AplicaOrdem(ordem)
                .Select(l => l.ToApi())
                .ToLivroPaginado(paginacao);

            return Ok(livroPaginado);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Recupera o livro pelo id",
            Tags = new[] { "Livros" }
        )]
        [ProducesResponseType(statusCode: 200, Type = typeof(LivroApi))]
        [ProducesResponseType(statusCode: 500, Type = typeof(ErrorResponse))]
        [ProducesResponseType(statusCode: 404)]
        public IActionResult Recuperar(
            [SwaggerParameter("Id do Livro", Required = true)] int id)
        {
            var model = _repo.Find(id);
            if (model == null) return NotFound();
            return Ok(model);
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Adiciona um livro",
            Tags = new[] { "Livros" }
        )]
        [ProducesResponseType(statusCode: 201, Type = typeof(LivroApi))]
        [ProducesResponseType(statusCode: 500, Type = typeof(ErrorResponse))]
        [ProducesResponseType(statusCode: 400, Type = typeof(ErrorResponse))]
        public IActionResult Incluir([FromForm] LivroUpload model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ErrorResponse.FromModelState(ModelState));

            var livro = model.ToLivro();
            _repo.Incluir(livro);
            var uri = Url.Action("Recuperar", new { id = livro.Id });
            return Created(uri, livro.ToApi());
        }

        [HttpPut]
        [SwaggerOperation(
            Summary = "Modifica um livro",
            Tags = new[] { "Livros" }
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(statusCode: 500, Type = typeof(ErrorResponse))]
        [ProducesResponseType(statusCode: 400, Type = typeof(ErrorResponse))]
        public IActionResult Alterar([FromForm] LivroUpload model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ErrorResponse.FromModelState(ModelState));

            var livro = model.ToLivro();
            if (model.Capa == null)
            {
                livro.ImagemCapa = _repo.All
                    .Where(l => l.Id == livro.Id)
                    .Select(l => l.ImagemCapa)
                    .FirstOrDefault();
            }
            _repo.Alterar(livro);
            return Ok();
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Remove um livro",
            Tags = new[] { "Livros" }
        )]
        [ProducesResponseType(204)]
        [ProducesResponseType(statusCode: 500, Type = typeof(ErrorResponse))]
        [ProducesResponseType(404)]
        public IActionResult Remover([SwaggerParameter("Id do livro")] int id)
        {
            var model = _repo.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            _repo.Excluir(model);
            return NoContent();
        }

        [HttpGet("{id}/capa")]
        [SwaggerOperation(
            Summary = "Consulta a imagem de capa de um livro pelo id",
            Tags = new[] { "Livros" }
        )]
        [Produces("image/png")]
        public IActionResult ImagemCapa(int id)
        {
            byte[] img = _repo.All
                .Where(l => l.Id == id)
                .Select(l => l.ImagemCapa)
                .FirstOrDefault();
            if (img != null)
            {
                return File(img, "image/png");
            }
            return File("~/images/capas/capa-vazia.png", "image/png");
        }
    }
}
