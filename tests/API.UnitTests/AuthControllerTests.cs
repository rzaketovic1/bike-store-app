using API.Controllers;
using Core.Dtos;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace API.UnitTests
{
    public class AuthControllerTests
    {
        [Fact]
        public async Task Register_ShouldThrowKeyNotFoundException_WhenEmailAlreadyInUse()
        {
            // Arrange
            var userService = new Mock<IUserService>();
            var tokenService = new Mock<ITokenService>();

            userService
                .Setup(s => s.GetByEmailAsync("a@b.com"))
                .ReturnsAsync(new User { Email = "a@b.com" });

            var controller = new AuthController(userService.Object, tokenService.Object);

            var dto = new UserRegisterDto
            {
                Email = "a@b.com",
                Password = "Pass123!",
                DisplayName = "Ramiz"
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => controller.Register(dto));

            Assert.Equal("Email already in use", ex.Message);
        }


        [Fact]
        public async Task Register_ShouldReturnUserDtoWithToken_WhenRegistrationSuccessful()
        {
            // Arrange
            var userService = new Mock<IUserService>();
            var tokenService = new Mock<ITokenService>();

            userService
                .Setup(s => s.GetByEmailAsync("new@user.com"))
                .ReturnsAsync((User)null!);

            var createdUser = new User
            {
                Email = "new@user.com",
                DisplayName = "Ramiz"
            };

            userService
                .Setup(s => s.CreateAsync("new@user.com", "Pass123!", "Ramiz"))
                .ReturnsAsync(createdUser);

            tokenService
                .Setup(t => t.CreateToken(createdUser))
                .Returns("fake-jwt-token");

            var controller = new AuthController(userService.Object, tokenService.Object);

            var dto = new UserRegisterDto
            {
                Email = "new@user.com",
                Password = "Pass123!",
                DisplayName = "Ramiz"
            };

            // Act
            var result = await controller.Register(dto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<UserDto>>(result);
            var userDto = Assert.IsType<UserDto>(actionResult.Value);

            Assert.Equal("new@user.com", userDto.Email);
            Assert.Equal("Ramiz", userDto.DisplayName);
            Assert.Equal("fake-jwt-token", userDto.Token);
        }
    }
}
