using Alura.ListaLeitura.Modelos;
using Alura.WebAPI.WebApp.HttpClients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.WebApp.Controllers
{
    [Authorize]
    public class LivroController : Controller
    {
        private readonly LivroApiClient _api;

        public LivroController(LivroApiClient api)
        {
            _api = api;
        }

        [HttpGet]
        public IActionResult Novo()
        {
            return View(new LivroUpload());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Novo(LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                await _api.PostLivroAsync(model);
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ImagemCapa(int id)
        {
            byte[] img = await _api.GetCapaAsync(id);
            return File(img, "image/png");
        }

        [HttpGet]
        public async Task<IActionResult> Detalhes(int id)
        {
            var model = await RecuperaLivro(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model.ToApi().ToUpload());
        }

        public async Task<Livro> RecuperaLivro(int id)
        {
            var model = await _api.GetLivroAsync(id);
            return model.ToUpload().ToLivro();
        }

        public async Task<ActionResult<LivroUpload>> DetalhesJSONAction(int id)
        {
            var model = await RecuperaLivro(id);
            if (model == null)
            {
                return NotFound();
            }
            return model.ToModel();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Alterar Livro
        public async Task<IActionResult> Detalhes(LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                await _api.PutLivroAsync(model);
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remover(int id)
        {
            var model = await _api.GetLivroAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            await _api.DeleteLivroAsync(id);
            return RedirectToAction("Index", "Home");
        }
    }
}