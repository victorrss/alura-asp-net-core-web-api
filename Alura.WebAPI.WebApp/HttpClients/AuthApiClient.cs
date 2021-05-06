using Alura.ListaLeitura.Seguranca;
using System.Net.Http;
using System.Threading.Tasks;

namespace Alura.WebAPI.WebApp.HttpClients
{
    public class AuthApiClient
    {
        private readonly HttpClient _httpClient;

        public AuthApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<LoginResult> PostLoginAync(LoginModel model)
        {
            var res = await _httpClient.PostAsJsonAsync("login", model);
            return new LoginResult
            {
                Succeeded = res.IsSuccessStatusCode,
                Token = await res.Content.ReadAsStringAsync()
            };
        }

        public async Task<LoginResult> PostRegisterAsync(LoginModel model)
        {
            var res = await _httpClient.PostAsJsonAsync("login/register", model);
            return new LoginResult
            {
                Succeeded = res.IsSuccessStatusCode,
                Token = await res.Content.ReadAsStringAsync()
            };
        }
    }

    public class LoginResult
    {
        public bool Succeeded { get; set; }
        public string Token { get; set; }
    }
}
