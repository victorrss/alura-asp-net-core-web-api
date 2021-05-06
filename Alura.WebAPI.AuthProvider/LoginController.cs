using Alura.ListaLeitura.Seguranca;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.Services
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly SignInManager<Usuario> _signInManager;
        private readonly UserManager<Usuario> _userManager;

        public LoginController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Token([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await _signInManager.PasswordSignInAsync(model.Login, model.Password, true, true);
            if (result.Succeeded)
            {
                return Ok(CriarToken(model));
            }
            return Unauthorized();
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new Usuario { UserName = model.Login };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return Ok(CriarToken(model));
                }
                else
                {
                    string sMsgErro = string.Empty;
                    foreach (var err in result.Errors)
                        sMsgErro += err.Description + "\r\n";

                    return BadRequest(sMsgErro);
                }
            }
            return BadRequest();
        }

        private string CriarToken(LoginModel model)
        {
            // Criar o token (header + payload(direitos) + signature)
            var direitos = new[]
            {
                    new Claim(JwtRegisteredClaimNames.Sub, model.Login),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

            var chave = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("curso-alura-asp-net-core-auth-validation"));
            var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "Alura.WebApp",
                audience: "Insomnia",
                claims: direitos,
                signingCredentials: credenciais,
                expires: DateTime.Now.AddMinutes(30)
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenString;
        }
    }
}
