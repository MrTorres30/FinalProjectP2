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
    public class PresupuestosController : ControllerBase
    {
        private readonly IPresupuestoService _presupuestoService;
        public PresupuestosController(IPresupuestoService presupuestoService)
        {
            _presupuestoService = presupuestoService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            int usuarioId = GetUsuarioId();
            var presupuestos = await _presupuestoService.GetByUsuarioIdAsync(usuarioId);
            return Ok(presupuestos);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            int usuarioId = GetUsuarioId();
            var presupuesto = await _presupuestoService.GetByIdAsync(id, usuarioId);
            if (presupuesto == null) return NotFound("Presupuesto no encontrado.");
            return Ok(presupuesto);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearPresupuestoDto dto)
        {
            int usuarioId = GetUsuarioId();
            var nuevo = await _presupuestoService.CrearAsync(dto, usuarioId);
            if (nuevo == null) return BadRequest("Error al crear el presupuesto.");
            return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CrearPresupuestoDto dto)
        {
            int usuarioId = GetUsuarioId();
            var success = await _presupuestoService.ActualizarAsync(id, dto, usuarioId);
            if (!success) return NotFound("Presupuesto no encontrado o no pertenece al usuario.");
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            int usuarioId = GetUsuarioId();
            var success = await _presupuestoService.EliminarAsync(id, usuarioId);
            if (!success) return NotFound("Presupuesto no encontrado o no pertenece al usuario.");
            return NoContent();
        }
        private int GetUsuarioId()
        {
            var claimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claimId, out int id) ? id : 0;
        }
    }
}