using Core.Entities;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUsersRepository _userRepo;

        public UserService(IUsersRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _userRepo.GetByEmailAsync(email);
        }

        public async Task<User> CreateAsync(string email, string password, string displayName)
        {
            var user = new User
            {
                Email = email,
                DisplayName = displayName,
                PasswordHash = HashPassword(password)
            };
            await _userRepo.AddUserAsync(user);
            await _userRepo.SaveChangesAsync();
            return user;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            var user = await _userRepo.GetByEmailAsync(email);
            if (user == null || user.PasswordHash != HashPassword(password))
                return null;
            return user;
        }

        private string HashPassword(string password)
        {
            // Simple SHA256 hash (nije za produkciju, ali OK za demo)
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
