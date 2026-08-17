using GastosPersonales.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GastosPersonales.Application.Services
{
    public interface ITokenService
    {
        string GenerateToken(Usuario usuario);
    }
}
