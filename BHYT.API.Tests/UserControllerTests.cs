using AutoMapper;
using BHYT.API.Controllers;
using BHYT.API.Models.DbModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq.EntityFrameworkCore;
using Moq;
using System.Collections;
using BHYT.API.Models.DTOs;

namespace BHYT.API.Tests
{
    public class UserControllerTests
    {
        private readonly UserController _controller;
        private readonly Mock<BHYTDbContext> _mockContext;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IMapper> _mockMapper;

        [Fact]
        public async Task GetUsers_ReturnsEmptyList_WhenNoUsersExist()
        {
            var users = new List<User>();

            _mockContext.Setup(c => c.Users).ReturnsDbSet(users);

            var result = await _controller.GetUsers();


            var objectResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Null(objectResult.Value);

        }

        public UserControllerTests()
        {
            var options = new DbContextOptionsBuilder<BHYTDbContext>()
               .UseInMemoryDatabase(databaseName: "TestDatabase")
               .Options;
            _mockContext = new Mock<BHYTDbContext>(options);
            _mockConfiguration = new Mock<IConfiguration>();
            _mockMapper = new Mock<IMapper>();
            _controller = new UserController(_mockContext.Object, _mockConfiguration.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetUserRole_ReturnsOk_WhenUserExists()
        {
            // Arrange
            var username = "testUser";
            var roles = new List<Role>{
                new Role { Id = 1, Name = "admin" },
                // Add more roles as needed
            };
                    var accounts = new List<Account>{
                new Account { Id = 1, Username = "testUser", Password="password" },
                // Add more accounts as needed
            };
                    var users = new List<User>{
                new User { Id = 1, AccountId = 1, RoleId = 1 },
                // Add more users as needed
            };

            _mockContext.Setup(c => c.Roles).ReturnsDbSet(roles);
            _mockContext.Setup(c => c.Accounts).ReturnsDbSet(accounts);
            _mockContext.Setup(c => c.Users).ReturnsDbSet(users);

            // Act
            var result = await _controller.GetUserRole(username);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetUserRole_ReturnsConflict_WhenExceptionIsThrown()
        {
            _mockContext.Setup(c => c.Accounts).Throws<Exception>();

            var username = "testUser";

            var result = await _controller.GetUserRole(username);

            Assert.IsType<ConflictObjectResult>(result);
        }




    }

}