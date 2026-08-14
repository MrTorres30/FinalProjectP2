using System;
using System.Collections.Generic;
using System.Text;

namespace GastosPersonales.Application.DTOs
{
    public class PerfilUsuarioDto
    {
        public string Nombre { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string? NewPassword { get; set; } = String.Empty;
    }
}
