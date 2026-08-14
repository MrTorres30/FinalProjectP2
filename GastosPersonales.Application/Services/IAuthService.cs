using GastosPersonales.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace GastosPersonales.Application.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> RegistrarAsync(RegistroRequestDto request);
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
        Task<PerfilUsuarioDto?> ObtenerPerfilAsync(int usuarioId);
        Task<bool> ActualizarPerfilAsync(int usuarioId, PerfilUsuarioDto request);
    }
}
