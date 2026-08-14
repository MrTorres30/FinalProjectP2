using System;
using System.Collections.Generic;
using System.Text;

namespace GastosPersonales.Application.DTOs
{
    public class LoginRequestDto
    {
        public string Email { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
    }
}
