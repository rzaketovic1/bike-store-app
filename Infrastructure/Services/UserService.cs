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
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };
            await _userRepo.AddUserAsync(user);
            await _userRepo.SaveChangesAsync();
            return user;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            var user = await _userRepo.GetByEmailAsync(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;
            return user;
        }
    }
}
