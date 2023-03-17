using IteraEmpresaGrupos.Data;
using IteraEmpresaGrupos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IteraEmpresaGrupos.Services
{
    public class UserService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IteraLogService _logService;
        private readonly TokenService _tokenService;


        public UserService(IServiceScopeFactory serviceScopeFactory, IteraLogService logService, TokenService tokenService)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logService = logService;
            _tokenService = tokenService;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await dbContext.Users.ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> GetUserByNameAsync(string name)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await dbContext.Users.FirstOrDefaultAsync(u => u.Name == name);
        }

        public async Task<User> GetUserByNameAndPasswordAsync(string name, string password)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            using var sha256 = SHA256.Create();
            var hashedPassword = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            password = Convert.ToBase64String(hashedPassword);
            return await dbContext.Users.FirstOrDefaultAsync(u => u.Name == name && u.Password == password);
        }


        public async Task<ActionResult<User>> CreateUserAsync(User user)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Verificar se já existe um usuário com o mesmo email
            var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser != null)
            {
                throw new ArgumentException($"A user with email {user.Email} already exists.");
            }

            // Hash da senha usando SHA256
            using var sha256 = SHA256.Create();
            var hashedPassword = sha256.ComputeHash(Encoding.UTF8.GetBytes(user.Password));
            user.Password = Convert.ToBase64String(hashedPassword);

            user.LastUpdate = DateTime.UtcNow;
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
            await _logService.CreateLogAsync(new Log { Message = $"User {user.Id} created" });

            return user;
        }
        public async Task<User> UpdateUserAsync(int id, User userIn)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var userToUpdate = await dbContext.Users.FindAsync(id);

            if (userToUpdate == null)
            {
                throw new Exception("User não encontrado(a)");
            }

            // Verificar se os valores foram atualizados
            bool valuesUpdated = false;
            if (userToUpdate.Name != userIn.Name)
            {
                userToUpdate.Name = userIn.Name;
                valuesUpdated = true;
            }
            if (userToUpdate.Email != userIn.Email)
            {
                userToUpdate.Email = userIn.Email;
                valuesUpdated = true;
            }
            if (userToUpdate.Password != userIn.Password)
            {
                userToUpdate.Password = userIn.Password;
                valuesUpdated = true;
            }

            if (valuesUpdated)
            {
                userToUpdate.LastUpdate = DateTime.UtcNow;
                await dbContext.SaveChangesAsync();
                await _logService.CreateLogAsync(new Log { Message = $"User {id} updated" });
            }

            return userToUpdate;
        }

        public async Task<User> AuthenticateAsync(string name, string password)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            using var sha256 = SHA256.Create();
            var hashedPassword = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var base64Password = Convert.ToBase64String(hashedPassword);

            var user = await dbContext.Users.SingleOrDefaultAsync(u => u.Name == name && u.Password == base64Password);

            return user;
        }
        public async Task RemoveUserAsync(int id)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var userToRemove = await dbContext.Users.FindAsync(id);

            if (userToRemove == null)
            {
                throw new Exception("User não encontrado(a)");
            }

            dbContext.Users.Remove(userToRemove);
            await dbContext.SaveChangesAsync();
            await _logService.CreateLogAsync(new Log { Message = $"User {id} removed" });
        }

        public async Task<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Token inválido");
            }
            int userId = Convert.ToInt32(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var newToken = _tokenService.GenerateToken(userId);
            var newTokenHandler = new JwtSecurityTokenHandler();
            var newPrincipal = newTokenHandler.ValidateToken(newToken, tokenValidationParameters, out securityToken);

            return newPrincipal;
        }
    }
}