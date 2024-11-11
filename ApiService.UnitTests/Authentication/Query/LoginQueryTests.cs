using Application.Common.Interfaces.Authentication;
using Application.Common.Models;
using Application.Common.Services;
using Application.Features.Authentication.Query.Login;
using Domain.Entities;
using Moq;


namespace ApiService.UnitTests.Authentication.Query
{
    public class LoginQueryHandlerTests
    {
        private readonly Mock<IAuthRepository> _authRepository;
        private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
        private readonly LoginQueryHandler _handler;

        public LoginQueryHandlerTests()
        {
            _authRepository = new Mock<IAuthRepository>();
            _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
            _handler = new LoginQueryHandler(_jwtTokenGeneratorMock.Object, _authRepository.Object);
        }
        [Fact]
        public async Task Should_Return_Error_When_Password_Is_Incorrect()
        {
            // Arrange
            var query = new LoginQuery("Sprtybi@Gmail.com", "incorrectPassword");

            var incorrectHashedPassword = Hash.HashPassword("incorrectPassword");

            _authRepository.Setup(repo => repo.Authenticate(query.Email, incorrectHashedPassword))
                .ReturnsAsync(new Result_VM<User> { Code = -1 , Message = "Username or Password is wrong." });

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(-1, result.Code);
            Assert.Equal("Username or Password is wrong.", result.Message);
        }

        [Fact]
        public async Task Should_Return_Token_When_Login_Successful()
        {
            // Arrange
            var query = new LoginQuery("Sprtybi@Gmail.com", "2260");
            var user = new User { Email = "Sprtybi@Gmail.com", PasswordHash = "UBgenv8txCdxSGKGGIuPLrtL0dXbt9vqvIs5K/VzrUA=" };
            var token = "jwt-token";
            var HashedPassword = Hash.HashPassword("2260");

            _authRepository.Setup(repo => repo.Authenticate(query.Email, HashedPassword))
                .ReturnsAsync(new Result_VM<User> { Result = user });

            _jwtTokenGeneratorMock.Setup(generator => generator.GenerateToken(user))
                .Returns(token);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Code);
            Assert.Equal(token, result.Result);
        }
    }

}
