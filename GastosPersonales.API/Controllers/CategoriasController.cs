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
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;
        public CategoriasController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            int usuarioId = GetUsuarioId();
            var categorias = await _categoriaService.GetByUsuarioIdAsync(usuarioId);
            return Ok(categorias);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            int usuarioId = GetUsuarioId();
            var categoria = await _categoriaService.GetByIdAsync(id, usuarioId);
            if (categoria == null) return NotFound("Categoría no encontrada.");
            return Ok(categoria);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearCategoriaDto dto)
        {
            int usuarioId = GetUsuarioId();
            var nueva = await _categoriaService.CrearAsync(dto, usuarioId);
            if (nueva == null) return BadRequest("Error al crear la categoría.");
            return CreatedAtAction(nameof(GetById), new { id = nueva.Id }, nueva);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CrearCategoriaDto dto)
        {
            int usuarioId = GetUsuarioId();
            var success = await _categoriaService.ActualizarAsync(id, dto, usuarioId);
            if (!success) return NotFound("Categoría no encontrada o no pertenece al usuario.");
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            int usuarioId = GetUsuarioId();
            var success = await _categoriaService.EliminarAsync(id, usuarioId);
            if (!success) return NotFound("Categoría no encontrada o no pertenece al usuario.");
            return NoContent();
        }
        private int GetUsuarioId()
        {
            var claimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claimId, out int id) ? id : 0;
        }
    }
}