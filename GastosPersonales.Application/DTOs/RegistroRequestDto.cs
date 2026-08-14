using System;
using System.Collections.Generic;
using System.Text;

namespace GastosPersonales.Application.DTOs
{
    public class RegistroRequestDto
    {
        public string Nombre {get; set;} = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
    }   
}
