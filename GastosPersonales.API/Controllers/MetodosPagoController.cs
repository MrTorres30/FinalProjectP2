using GastosPersonales.Application.DTOs;
using GastosPersonales.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
namespace GastosPersonales.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MetodosPagoController : ControllerBase
    {
        private readonly IMetodoPagoService _metodoPagoService;
        public MetodosPagoController(IMetodoPagoService metodoPagoService)
        {
            _metodoPagoService = metodoPagoService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            int usuarioId = GetUsuarioId();
            var metodos = await _metodoPagoService.GetByUsuarioIdAsync(usuarioId);
            return Ok(metodos);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            int usuarioId = GetUsuarioId();
            var metodo = await _metodoPagoService.GetByIdAsync(id, usuarioId);
            if (metodo == null) return NotFound("Método de pago no encontrado.");
            return Ok(metodo);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearMetodoPagoDto dto)
        {
            int usuarioId = GetUsuarioId();
            var nuevo = await _metodoPagoService.CrearAsync(dto, usuarioId);
            if (nuevo == null) return BadRequest("Error al crear el método de pago.");
            return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CrearMetodoPagoDto dto)
        {
            int usuarioId = GetUsuarioId();
            var success = await _metodoPagoService.ActualizarAsync(id, dto, usuarioId);
            if (!success) return NotFound("Método de pago no encontrado o no pertenece al usuario.");
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            int usuarioId = GetUsuarioId();
            var success = await _metodoPagoService.EliminarAsync(id, usuarioId);
            if (!success) return NotFound("Método de pago no encontrado o no pertenece al usuario.");
            return NoContent();
        }
        private int GetUsuarioId()
        {
            var claimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claimId, out int id) ? id : 0;
        }
    }
}