using AutoMapper;
using BHYT.API.Controllers;
using BHYT.API.Models.DbModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHYT.API.Tests
{
    public class CustomerPolicyControllerTests
    {
        private readonly CustomerPolicyController _controller;
        private readonly Mock<BHYTDbContext> _mockContext;
        private readonly Mock<IMapper> _mockMapper;

        public CustomerPolicyControllerTests()
        {

            var options = new DbContextOptionsBuilder<BHYTDbContext>()
               .UseInMemoryDatabase(databaseName: "TestDatabase")
               .Options;
            _mockContext = new Mock<BHYTDbContext>(options);
            _mockMapper = new Mock<IMapper>();
            _mockMapper.Setup(m => m.ConfigurationProvider).Returns(() => new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CustomerPolicy, CustomerPolicyDTO>();
            }));

            _controller = new CustomerPolicyController(_mockContext.Object, _mockMapper.Object);

        }

        [Fact]
        public void GetCustomerPolicy_ReturnsNotFound_WhenNoPolicyExists()
        {
            // Arrange
            var testId = 1;

            // Create a list of customer policies
            var customerPolicies = new List<CustomerPolicy>();

            // Create a mock DbSet
            var mockSet = new Mock<DbSet<CustomerPolicy>>();
            mockSet.As<IQueryable<CustomerPolicy>>().Setup(m => m.Provider).Returns(customerPolicies.AsQueryable().Provider);
            mockSet.As<IQueryable<CustomerPolicy>>().Setup(m => m.Expression).Returns(customerPolicies.AsQueryable().Expression);
            mockSet.As<IQueryable<CustomerPolicy>>().Setup(m => m.ElementType).Returns(customerPolicies.AsQueryable().ElementType);
            mockSet.As<IQueryable<CustomerPolicy>>().Setup(m => m.GetEnumerator()).Returns(customerPolicies.GetEnumerator());

            // Create a mock context
            _mockContext.Setup(c => c.CustomerPolicies).Returns(mockSet.Object);

            // Create the controller with the mock context and mapper
            var _controller = new CustomerPolicyController(_mockContext.Object, _mockMapper.Object);

            // Act
            var result = _controller.GetCustomerPolicy(testId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void GetCustomerPolicy_ReturnsData_WhenPolicyExists()
        {
            // Arrange
            var testId = 1;
            var customerPolicy = new CustomerPolicy { Id = testId };

            // Create a list of customer policies
            var customerPolicies = new List<CustomerPolicy> { customerPolicy };

            // Create a mock DbSet
            var mockSet = new Mock<DbSet<CustomerPolicy>>();
            mockSet.As<IQueryable<CustomerPolicy>>().Setup(m => m.Provider).Returns(customerPolicies.AsQueryable().Provider);
            mockSet.As<IQueryable<CustomerPolicy>>().Setup(m => m.Expression).Returns(customerPolicies.AsQueryable().Expression);
            mockSet.As<IQueryable<CustomerPolicy>>().Setup(m => m.ElementType).Returns(customerPolicies.AsQueryable().ElementType);
            mockSet.As<IQueryable<CustomerPolicy>>().Setup(m => m.GetEnumerator()).Returns(customerPolicies.GetEnumerator());

            // Create a mock context
            _mockContext.Setup(c => c.CustomerPolicies).Returns(mockSet.Object);

            // Create the controller with the mock context and mapper
            var _controller = new CustomerPolicyController(_mockContext.Object, _mockMapper.Object);

            // Act
            var result = _controller.GetCustomerPolicy(testId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var expectedJson = "{\"customerPolicy\":{\"Guid\":null,\"StartDate\":null,\"CreatedDate\":null,\"EndDate\":null,\"PremiumAmount\":null,\"PaymentOption\":null,\"CoverageType\":null,\"DeductibleAmount\":null,\"BenefitId\":null,\"InsuranceId\":null,\"LatestUpdate\":null,\"Description\":null,\"Status\":null,\"Company\":null}}";
            var actualJson = JsonConvert.SerializeObject(okResult.Value);
            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public void GetCustomerPolicy_ReturnsConflict_WhenExceptionOccurs()
        {
            // Arrange
            var testId = 1;

            // Create a mock DbSet
            var mockSet = new Mock<DbSet<CustomerPolicy>>();
            mockSet.As<IQueryable<CustomerPolicy>>().Setup(m => m.Provider).Throws<Exception>();
            mockSet.As<IQueryable<CustomerPolicy>>().Setup(m => m.Expression).Throws<Exception>();
            mockSet.As<IQueryable<CustomerPolicy>>().Setup(m => m.ElementType).Throws<Exception>();
            mockSet.As<IQueryable<CustomerPolicy>>().Setup(m => m.GetEnumerator()).Throws<Exception>();

            // Create a mock context
            _mockContext.Setup(c => c.CustomerPolicies).Returns(mockSet.Object);

            // Create the controller with the mock context and mapper
            var _controller = new CustomerPolicyController(_mockContext.Object, _mockMapper.Object);

            // Act
            var result = _controller.GetCustomerPolicy(testId);

            // Assert
            Assert.IsType<ConflictObjectResult>(result);
        }
    }
}
