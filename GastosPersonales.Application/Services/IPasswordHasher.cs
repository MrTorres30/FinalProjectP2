using GastosPersonales.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GastosPersonales.Application.Services
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool Verify (string Password, string hashedPassword);
    }
}
