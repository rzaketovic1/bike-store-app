using Core.Entities;

namespace Application.Interfaces;

public interface IUserService
{
    Task<User?> GetByEmailAsync(string email);
    Task<User> CreateAsync(string email, string password, string displayName);
    Task<User> AuthenticateAsync(string email, string password);
}
