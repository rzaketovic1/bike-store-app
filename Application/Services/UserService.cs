using Application.Common.Exceptions;
using Application.Interfaces;
using Core.Entities;
using Core.Interfaces;

namespace Application.Services;

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
        var existingUser = await _userRepo.GetByEmailAsync(email);
        if (existingUser != null)
        {
            throw new ConflictException("Email already in use");
        }

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

    public async Task<User> AuthenticateAsync(string email, string password)
    {
        var user = await _userRepo.GetByEmailAsync(email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        return user;
    }
}
