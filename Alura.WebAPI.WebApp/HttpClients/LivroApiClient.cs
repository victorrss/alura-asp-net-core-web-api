﻿using Alura.ListaLeitura.Modelos;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Lista = Alura.ListaLeitura.Modelos.ListaLeitura;

namespace Alura.WebAPI.WebApp.HttpClients
{
    public class LivroApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _accessor;

        public LivroApiClient(HttpClient httpClient, IHttpContextAccessor accessor)
        {
            _httpClient = httpClient;
            _accessor = accessor;
        }

        public async Task<LivroApi> GetLivroAsync(int id)
        {
            AddBearerToken();
            HttpResponseMessage response = await _httpClient.GetAsync($"livros/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<LivroApi>();
        }

        public async Task<byte[]> GetCapaAsync(int id)
        {
            AddBearerToken();
            HttpResponseMessage response = await _httpClient.GetAsync($"livros/{id}/capa");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task DeleteLivroAsync(int id)
        {
            AddBearerToken();
            HttpResponseMessage response = await _httpClient.DeleteAsync($"livros/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<Lista> GetListaLeituraAsync(TipoListaLeitura tipo)
        {
            AddBearerToken();
            HttpResponseMessage response = await _httpClient.GetAsync($"listasleitura/{tipo}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<Lista>();
        }

        public async Task PostLivroAsync(LivroUpload model)
        {
            AddBearerToken();
            HttpContent content = CreateMultipartFormDataContent(model);
            HttpResponseMessage response = await _httpClient.PostAsync("livros", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task PutLivroAsync(LivroUpload model)
        {
            AddBearerToken();
            HttpContent content = CreateMultipartFormDataContent(model);
            HttpResponseMessage response = await _httpClient.PutAsync("livros", content);
            response.EnsureSuccessStatusCode();
        }

        private HttpContent CreateMultipartFormDataContent(LivroUpload model)
        {
            var content = new MultipartFormDataContent();

            // Obrigatórios
            content.Add(new StringContent(model.Titulo), EnvolveComAspas("titulo"));
            content.Add(new StringContent(model.Lista.ParaString()), EnvolveComAspas("lista"));

            if (!string.IsNullOrEmpty(model.Subtitulo))
                content.Add(new StringContent(model.Subtitulo), EnvolveComAspas("subtitulo"));

            if (!string.IsNullOrEmpty(model.Resumo))
                content.Add(new StringContent(model.Resumo), EnvolveComAspas("resumo"));

            if (!string.IsNullOrEmpty(model.Autor))
                content.Add(new StringContent(model.Autor), EnvolveComAspas("autor"));

            if (model.Id > 0)
                content.Add(new StringContent(model.Id.ToString()), EnvolveComAspas("id"));

            if (model.Capa != null)
            {
                var imgContent = new ByteArrayContent(model.Capa.ConvertToBytes());
                imgContent.Headers.Add("content-type", "image/png");
                content.Add(imgContent, EnvolveComAspas("capa"), EnvolveComAspas("capa.png"));
            }

            return content;
        }
        private string EnvolveComAspas(string valor)
        {
            return $"\"{valor}\"";
        }

        private void AddBearerToken()
        {
            var token = _accessor.HttpContext.User.Claims.First(c => c.Type == "Token").Value;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
