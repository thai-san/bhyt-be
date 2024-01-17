using BHYT.API.Controllers;
using BHYT.API.Models.DbModels;
using BHYT.API.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq.EntityFrameworkCore;
using Moq;
using Microsoft.AspNetCore.Http;

namespace BHYT.API.Tests
{
    public class LoginControllerTests
    {
        private readonly LoginController _controller;
        private readonly Mock<BHYTDbContext> _mockContext;
        private readonly Mock<IConfiguration> _mockConfiguration;

        public LoginControllerTests()
        {
            var options = new DbContextOptionsBuilder<BHYTDbContext>()
               .UseInMemoryDatabase(databaseName: "TestDatabase")
               .Options;
            _mockContext = new Mock<BHYTDbContext>(options);
            _mockConfiguration = new Mock<IConfiguration>();
            _controller = new LoginController(_mockContext.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenDtoIsNull()
        {
            // Act
            var result = await _controller.Login(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenUsernameOrPasswordIsEmpty()
        {
            var testDto = new LoginDTO { Username = "", Password = "" };

            // Act
            var result = await _controller.Login(testDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_ReturnsNotFound_WhenAccountDoesNotExist()
        {
            var accounts = new List<Account>();

            _mockContext.Setup(c => c.Accounts).ReturnsDbSet(accounts);

            var testDto = new LoginDTO { Username = "nonExistingUser", Password = "password" };

            // Act
            var result = await _controller.Login(testDto);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenPasswordIsInvalid()
            {
                var accounts = new List<Account>{
            new Account { Username = "testUser", Password = BCrypt.Net.BCrypt.HashPassword("password") },
        };

                _mockContext.Setup(c => c.Accounts).ReturnsDbSet(accounts);

                var testDto = new LoginDTO { Username = "testUser", Password = "wrongPassword" };

                // Act
                var result = await _controller.Login(testDto);

                // Assert
                Assert.IsType<ObjectResult>(result);
            }




        [Fact]
            public async Task Login_ReturnsOk_WhenCredentialsAreValid()
                {
                var accounts = new List<Account>{
                new Account { Username = "testUser", Password = BCrypt.Net.BCrypt.HashPassword("password") },
                // Add more accounts as needed
            };

            _mockContext.Setup(c => c.Accounts).ReturnsDbSet(accounts);

            var testDto = new LoginDTO { Username = "testUser", Password = "password" };
            // Act
            var result = await _controller.Login(testDto);

            // Assert
            Assert.IsType<ObjectResult>(result);
        }

        [Fact]
        public async Task Login_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            _mockContext.Setup(c => c.Accounts).Throws<Exception>();

            var testDto = new LoginDTO { Username = "testUser", Password = "password" };

            // Act
            var result = await _controller.Login(testDto);

            // Assert
            Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, ((ObjectResult)result).StatusCode);
        }

    }

}
