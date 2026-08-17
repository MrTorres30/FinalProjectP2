using GastosPersonales.Application.DTOs;
using GastosPersonales.Domain.Entities;
using GastosPersonales.Domain.Repositories;
using System;
using System.Threading.Tasks;

namespace GastosPersonales.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public AuthService(
            IUsuarioRepository usuarioRepository, 
            IPasswordHasher passwordHasher, 
            ITokenService tokenService)
        {
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        //  Login y registro de usuario 
        public async Task<LoginResponseDto?> RegistrarAsync(RegistroRequestDto request)
        {
            var usuarioExistente = await _usuarioRepository.GetByEmailAsync(request.Email);
            if (usuarioExistente != null)
            {
                return null;
            }

            var passwordHash = _passwordHasher.HashPassword(request.Password);

            var nuevoUsuario = new Usuario
            {
                Nombre = request.Nombre,
                Email = request.Email,
                Password = passwordHash,
                FechaRegistro = DateTime.UtcNow
            };

            await _usuarioRepository.AddAsync(nuevoUsuario);
            var token = _tokenService.GenerateToken(nuevoUsuario);

            return new LoginResponseDto
            {
                Nombre = nuevoUsuario.Nombre,
                Email = nuevoUsuario.Email,
                Token = token
            };
        }

        // inicio de sesión de usuario
        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);
            if (usuario == null) return null;

            var esPasswordValido = _passwordHasher.Verify(request.Password, usuario.Password);
            if (!esPasswordValido) return null;

            var token = _tokenService.GenerateToken(usuario);

            return new LoginResponseDto
            {
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Token = token
            };
        }

        // obtener perfil 
        public async Task<PerfilUsuarioDto?> ObtenerPerfilAsync(int usuarioId)
        {
            var usuario = await _usuarioRepository.GetUsuarioAsync(usuarioId);
            if (usuario == null) return null;

            return new PerfilUsuarioDto
            {
                Nombre = usuario.Nombre,
                Email = usuario.Email
            };
        }

        // editar perfil 
        public async Task<bool> ActualizarPerfilAsync(int usuarioId, PerfilUsuarioDto request)
        {
            var usuario = await _usuarioRepository.GetUsuarioAsync(usuarioId);
            if (usuario == null) return false;

            usuario.Nombre = request.Nombre;

            if (!string.IsNullOrEmpty(request.NewPassword))
            {
                usuario.Password = _passwordHasher.HashPassword(request.NewPassword);
            }

            await _usuarioRepository.UpdateAsync(usuario);
            return true;
        }
    }
}

