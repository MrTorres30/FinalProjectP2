using GastosPersonales.Application.DTOs;
using GastosPersonales.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
namespace GastosPersonales.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistroRequestDto request)
        {
            var result = await _authService.RegistrarAsync(request);
            if (result == null)
                return BadRequest("El correo electrónico ya está registrado.");
            return Ok(result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request);
            if (result == null)
                return Unauthorized("Credenciales inválidas.");
            return Ok(result);
        }
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            int usuarioId = GetUsuarioId();
            var perfil = await _authService.ObtenerPerfilAsync(usuarioId);
            if (perfil == null) return NotFound("Usuario no encontrado.");
            return Ok(perfil);
        }
        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] PerfilUsuarioDto request)
        {
            int usuarioId = GetUsuarioId();
            var success = await _authService.ActualizarPerfilAsync(usuarioId, request);
            if (!success) return BadRequest("No se pudo actualizar el perfil.");
            return Ok("Perfil actualizado exitosamente.");
        }
        private int GetUsuarioId()
        {
            var claimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claimId, out int id) ? id : 0;
        }
    }
}