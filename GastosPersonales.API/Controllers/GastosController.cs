using GastosPersonales.Application.DTOs;
using GastosPersonales.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
namespace GastosPersonales.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class GastosController : ControllerBase
    {
        private readonly IGastoService _gastoService;
        public GastosController(IGastoService gastoService)
        {
            _gastoService = gastoService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] FiltroGastoDto filtro)
        {
            int usuarioId = GetUsuarioId();
            var gastos = await _gastoService.GetByUsuarioIdAsync(usuarioId, filtro);
            return Ok(gastos);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            int usuarioId = GetUsuarioId();
            var gasto = await _gastoService.GetByIdAsync(id, usuarioId);
            if (gasto == null) return NotFound("Gasto no encontrado.");
            return Ok(gasto);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearGastoDto dto)
        {
            int usuarioId = GetUsuarioId();
            var nuevo = await _gastoService.CrearAsync(dto, usuarioId);
            if (nuevo == null) return BadRequest("Error al registrar el gasto. Verifica que la categoría y método de pago pertenezcan a tu usuario.");
            return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CrearGastoDto dto)
        {
            int usuarioId = GetUsuarioId();
            var success = await _gastoService.ActualizarAsync(id, dto, usuarioId);
            if (!success) return NotFound("Gasto no encontrado o no pertenece al usuario.");
            var gastoActualizado = await _gastoService.GetByIdAsync(id, usuarioId);
            return Ok(gastoActualizado);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            int usuarioId = GetUsuarioId();
            var success = await _gastoService.EliminarAsync(id, usuarioId);
            if (!success) return NotFound("Gasto no encontrado o no pertenece al usuario.");
            return NoContent();
        }
        [HttpPost("importar-excel")]
        public async Task<IActionResult> ImportarExcel(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
                return BadRequest("Por favor, sube un archivo Excel válido.");
            int usuarioId = GetUsuarioId();
            using var stream = archivo.OpenReadStream();
            var resultados = await _gastoService.ImportarDesdeExcelAsync(stream, usuarioId);
            return Ok(resultados);
        }
        private int GetUsuarioId()
        {
            var claimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claimId, out int id) ? id : 0;
        }
    }
}