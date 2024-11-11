using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Users;
using Application.Common.Models;
using Application.Features.Users.Query.GetUsers;
using Domain.Entities;
using Moq;


namespace ApiService.UnitTests.UserTests.Query
{
    public class GetUsersQueryTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
        private readonly GetUsersQueryHandler _handler;

        public GetUsersQueryTests()
        {
            // Mocking IUserRepository and IJwtTokenGenerator
            _userRepositoryMock = new Mock<IUserRepository>();
            _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();

            // Initializing the handler with the mocked dependencies
            _handler = new GetUsersQueryHandler(_userRepositoryMock.Object, _jwtTokenGeneratorMock.Object);
        }

        [Fact]
        public async Task Should_Return_Users_When_Users_Exist()
        {
            // Arrange
            var users = new List<User>
            {
            new User { Id = 1, Username = "Sprtybi", FullName = "Sepehr Tayebi", Email = "Sprtybi@Gmail.com" },
            new User { Id = 2, FullName = "User 2", Email = "user2@example.com" }
            };

            _userRepositoryMock.Setup(repo => repo.GetUsers())
                .ReturnsAsync(new Result_VM<IEnumerable<User>> { Result = users });

            var query = new GetUsersQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Code);
            Assert.Equal(2, result.Result.Count);
        }

        [Fact]
        public async Task Should_Return_Error_When_No_Users_Found()
        {
            // Arrange
            _userRepositoryMock.Setup(repo => repo.GetUsers())
                .ReturnsAsync(new Result_VM<IEnumerable<User>> { Result = null });

            var query = new GetUsersQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(-1, result.Code);
            Assert.Equal("There is no user found", result.Message);
        }
    }

}