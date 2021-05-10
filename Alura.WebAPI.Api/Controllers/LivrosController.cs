﻿using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Persistencia;
using Alura.WebAPI.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;

namespace Alura.ListaLeitura.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class LivrosController : ControllerBase
    {
        private readonly IRepository<Livro> _repo;

        public LivrosController(IRepository<Livro> repository)
        {
            _repo = repository;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Recupera todos os livros.")]
        [ProducesResponseType(statusCode: 200, Type = typeof(List<LivroApi>))]
        [ProducesResponseType(statusCode: 500, Type = typeof(ErrorResponse))]
        public IActionResult Todos()
        {
            var lista = _repo.All.Select(l => l.ToApi()).ToList();
            return Ok(lista);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(statusCode: 200, Type = typeof(LivroApi))]
        [ProducesResponseType(statusCode: 500, Type = typeof(ErrorResponse))]
        [ProducesResponseType(statusCode: 404)]
        [SwaggerOperation(Summary = "Consulta um livro pelo id")]
        public IActionResult Recuperar([SwaggerParameter("Id do Livro")] int id)
        {
            var model = _repo.Find(id);
            if (model == null) return NotFound();
            return Ok(model.ToApi());
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Adiciona um livro")]
        [ProducesResponseType(statusCode: 200, Type = typeof(Livro))]
        [ProducesResponseType(statusCode: 500, Type = typeof(ErrorResponse))]
        [ProducesResponseType(statusCode: 400)]
        public IActionResult Incluir([FromForm] LivroUpload model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var livro = model.ToLivro();
            _repo.Incluir(livro);
            var uri = Url.Action("Recuperar", new { id = livro.Id });
            return Created(uri, livro);
        }

        [HttpPut]
        [SwaggerOperation(Summary = "Modifica um livro")]
        [ProducesResponseType(statusCode: 200, Type = typeof(Livro))]
        [ProducesResponseType(statusCode: 500, Type = typeof(ErrorResponse))]
        [ProducesResponseType(statusCode: 400)]
        public IActionResult Alterar([FromForm] LivroUpload model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

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
        [SwaggerOperation(Summary = "Remove um livro")]
        [ProducesResponseType(204)]
        [ProducesResponseType(statusCode: 500, Type = typeof(ErrorResponse))]
        [ProducesResponseType(404)]
        public IActionResult Remover(int id)
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
        [SwaggerOperation(Summary = "Consulta a imagem de capa de um livro pelo id")]
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
