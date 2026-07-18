using Application.Dtos;
using Core.Entities;

namespace Application.Mappings;

public static class UserMappingExtensions
{
    public static UserDto ToDto(this User user, string token)
    {
        return new UserDto
        {
            Email = user.Email,
            DisplayName = user.DisplayName,
            Token = token
        };
    }
}
