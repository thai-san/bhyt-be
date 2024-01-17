using AutoMapper;
using BHYT.API.Controllers;
using BHYT.API.Models.DbModels;
using BHYT.API.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;

namespace BHYT.API.Tests
{
    public class RegisterControllerTests
    {
        private readonly RegisterController _controller;
        private readonly Mock<BHYTDbContext> _mockContext;
        private readonly Mock<IMapper> _mockMapper;

        public RegisterControllerTests()
        {
            var options = new DbContextOptionsBuilder<BHYTDbContext>()
               .UseInMemoryDatabase(databaseName: "TestDatabase")
               .Options;
            _mockContext = new Mock<BHYTDbContext>(options);
            _mockMapper = new Mock<IMapper>();
            _mockMapper.Setup(m => m.ConfigurationProvider).Returns(() => new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Account, RegisterDTO>();
                cfg.CreateMap<User, RegisterDTO>();
            }));
            _controller = new RegisterController(_mockContext.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenUserExists()
        {
            var accounts = new List<Account>{
                new Account { Username = "existingUser", Password = "password" },
                // Add more accounts as needed
            };

            _mockContext.Setup(c => c.Accounts).ReturnsDbSet(accounts);

            var testDto = new RegisterDTO { Username = "existingUser", Email = "test@test.com", Password = "password" };
            // Act
            var result = await _controller.register(testDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenEmailExists()
        {
            var accounts = new List<Account>{
                new Account { Username = "existingUser", Password = "password" },
                // Add more accounts as needed
            };
            var users = new List<User>{
                new User { Email = "existingEmail@test.com" },
                // Add more users as needed
            };

            _mockContext.Setup(c => c.Accounts).ReturnsDbSet(accounts);
            _mockContext.Setup(c => c.Users).ReturnsDbSet(users);

            var testDto = new RegisterDTO { Username = "newUser", Email = "existingEmail@test.com", Password = "password" };
            // Act
            var result = await _controller.register(testDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_ReturnsOk_WhenRegistrationIsSuccessful()
        {
            var accounts = new List<Account>().AsQueryable();
            var users = new List<User>().AsQueryable();

            _mockContext.Setup(c => c.Accounts).ReturnsDbSet(accounts);
            _mockContext.Setup(c => c.Users).ReturnsDbSet(users);

            var testDto = new RegisterDTO { Username = "newUser", Email = "newEmail@test.com", Password = "password" };
            // Act
            var result = await _controller.register(testDto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Register_ReturnsConflict_WhenExceptionIsThrown()
        {
            var accounts = new List<Account>().AsQueryable();
            var users = new List<User>().AsQueryable();

            _mockContext.Setup(c => c.Accounts).ReturnsDbSet(accounts);
            _mockContext.Setup(c => c.Users).ReturnsDbSet(users);
            _mockContext.Setup(c => c.SaveChangesAsync(default)).Throws<Exception>();

            var testDto = new RegisterDTO { Username = "newUser", Email = "newEmail@test.com", Password = "password" };
            // Act
            var result = await _controller.register(testDto);

            // Assert
            Assert.IsType<ConflictObjectResult>(result);
        }


    }
}
