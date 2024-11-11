using Application.Common.Interfaces.RabbitMQ;
using Application.Common.Interfaces.Users;
using Application.Common.Models;
using Application.Features.RabbitMQ.Command;
using Domain.Entities;
using Moq;


namespace ApiService.UnitTests.RabbitMQ.Command
{
    public class SendUserToQueueCommandTests
    {
        private readonly Mock<IRabbitMqService> _rabbitMqServiceMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly SendUserToQueueCommandHandler _handler;

        public SendUserToQueueCommandTests()
        {
            _rabbitMqServiceMock = new Mock<IRabbitMqService>();
            _userRepositoryMock = new Mock<IUserRepository>();

            _handler = new SendUserToQueueCommandHandler(_rabbitMqServiceMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public async Task Should_Return_Error_When_User_Not_Found()
        {
            // Arrange
            var command = new SendUserToQueueCommand { UserId = 1 };
            var userResult = new Result_VM<User> { Result = null, Message = "User not found" };

            _userRepositoryMock.Setup(repo => repo.GetUserById(command.UserId))
                .ReturnsAsync(userResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userResult.Code, result.Code);
            Assert.Equal(userResult.Message, result.Message);
        }

        [Fact]
        public async Task Should_Return_Success_When_User_Found_And_Message_Sent()
        {
            // Arrange
            var command = new SendUserToQueueCommand { UserId = 1 };
            var user = new User { Id = 1, Username = "Sprtybi", FullName = "Sepehr Tayebi", Email = "Sprtybi@Gmail.com" };
            var userResult = new Result_VM<User> { Result = user };

            _userRepositoryMock.Setup(repo => repo.GetUserById(command.UserId))
                .ReturnsAsync(userResult);

            _rabbitMqServiceMock.Setup(rmq => rmq.SendMessageAsync(user))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Code);
            Assert.Equal("Message sent to queue successfully.", result.Message);
        }

        [Fact]
        public async Task Should_Return_Error_When_SendMessageToQueue_Fails()
        {
            // Arrange
            var command = new SendUserToQueueCommand { UserId = 1 };
            var user = new User { Id = 1, Username = "Sprtybi", FullName = "Sepehr Tayebi", Email = "Sprtybi@Gmail.com" };
            var userResult = new Result_VM<User> { Result = user };

            _userRepositoryMock.Setup(repo => repo.GetUserById(command.UserId))
                .ReturnsAsync(userResult);

            _rabbitMqServiceMock.Setup(rmq => rmq.SendMessageAsync(user))
                .ThrowsAsync(new Exception("Error sending message"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(-1, result.Code);
            Assert.Equal("Error sending message", result.Message);
        }
    }

}
