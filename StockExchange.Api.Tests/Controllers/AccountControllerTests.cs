using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using StockExchange.Api.Controllers;
using StockExchange.Core.Model;

namespace StockExchange.Api.Tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly AccountController accountController;

        private readonly Mock<UserManager<IdentityUser>> userManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> roleManagerMock;
        private readonly Mock<IConfiguration> configurationMock;

        private readonly Faker faker;

        public AccountControllerTests()
        {
            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            userManagerMock = new Mock<UserManager<IdentityUser>>(userStoreMock.Object,null, null, null, null, null, null, null, null);

            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            roleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStoreMock.Object, null, null, null, null);

            configurationMock = new Mock<IConfiguration>();

            accountController = new AccountController(userManagerMock.Object, roleManagerMock.Object, configurationMock.Object);

            faker = new Faker();
        }

        [Fact]
        public async Task Register_WhenUserCreatedSuccessfully_ReturnsOk()
        {
            // Arrange
            var model = new RegisterBroker { Username = faker.Internet.UserName(), Password = faker.Internet.Password() };

            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), model.Password))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await accountController.RegisterAsync(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var json = JsonConvert.SerializeObject(okResult.Value);
            dynamic obj = JsonConvert.DeserializeObject<dynamic>(json);

            Assert.Equal("User registered successfully", (string)obj.message);
        }

        [Fact]
        public async Task Register_WhenUserCreationFails_ReturnsBadRequest()
        {
            // Arrange
            var model = new RegisterBroker { Username = faker.Internet.UserName(), Password = faker.Internet.Password() };
            var identityErrors = new[] { new IdentityError { Description = "Error" } };

            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), model.Password))
                .ReturnsAsync(IdentityResult.Failed(identityErrors));

            // Act
            var result = await accountController.RegisterAsync(model);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AddRole_WhenRoleCreatedSuccessfully_ReturnsOk()
        {
            // Arrange
            roleManagerMock.Setup(x => x.RoleExistsAsync("Admin")).ReturnsAsync(false);
            roleManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await accountController.AddRoleAsync(new RoleModel { Role = "Admin" });

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var json = JsonConvert.SerializeObject(okResult.Value);
            dynamic obj = JsonConvert.DeserializeObject<dynamic>(json);

            Assert.Equal("Role added successfully", (string)obj.message);
        }

        [Fact]
        public async Task AddRole_WhenRoleAlreadyExists_ReturnsBadRequest()
        {
            // Arrange
            roleManagerMock.Setup(x => x.RoleExistsAsync("Admin")).ReturnsAsync(true);

            // Act
            var result = await accountController.AddRoleAsync(new RoleModel { Role = "Admin" });

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var json = JsonConvert.SerializeObject(badRequest.Value);
            dynamic obj = JsonConvert.DeserializeObject<dynamic>(json);

            Assert.Equal("Role already exists", (string)obj.error);
        }

        [Fact]
        public async Task AssignRole_WhenRoleAssignedSuccessfully_ReturnsOk()
        {
            // Arrange
            var user = new IdentityUser { UserName = faker.Internet.UserName() };
            var model = new BrokerRole { Username = faker.Internet.UserName(), Role = "Admin" };

            userManagerMock.Setup(x => x.FindByNameAsync(model.Username)).ReturnsAsync(user);
            userManagerMock.Setup(x => x.AddToRoleAsync(user, model.Role)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await accountController.AssignRoleAsync(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var json = JsonConvert.SerializeObject(okResult.Value);
            dynamic obj = JsonConvert.DeserializeObject<dynamic>(json);

            Assert.Equal("Role assigned successfully", (string)obj.message);
        }

        [Fact]
        public async Task AssignRole_WhenUserNotFound_ReturnsBadRequest()
        {
            // Arrange
            var model = new BrokerRole { Username = faker.Internet.UserName(), Role = "Admin" };
            userManagerMock.Setup(x => x.FindByNameAsync(model.Username)).ReturnsAsync((IdentityUser)null);

            // Act
            var result = await accountController.AssignRoleAsync(model);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var json = JsonConvert.SerializeObject(badRequest.Value);
            dynamic obj = JsonConvert.DeserializeObject<dynamic>(json);

            Assert.Equal("User not found", (string)obj.error);
        }

        [Fact]
        public async Task Login_WhenCredentialsAreValid_ReturnsToken()
        {
            // Arrange
            var user = new IdentityUser { UserName = faker.Internet.UserName() };
            var model = new BrokerLogin { Username = faker.Internet.UserName(), Password = faker.Internet.Password() };

            userManagerMock.Setup(x => x.FindByNameAsync(model.Username)).ReturnsAsync(user);
            userManagerMock.Setup(x => x.CheckPasswordAsync(user, model.Password)).ReturnsAsync(true);
            userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Admin" });

            configurationMock.Setup(x => x["Jwt:Issuer"]).Returns("TestIssuer");
            configurationMock.Setup(x => x["Jwt:ExpiryMinutes"]).Returns("30");
            configurationMock.Setup(x => x["Jwt:Key"]).Returns("supersecretkey1234567890abcdef12345678");

            // Act
            var result = await accountController.LoginAsync(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var json = JsonConvert.SerializeObject(okResult.Value);
            dynamic obj = JsonConvert.DeserializeObject<dynamic>(json);

            Assert.False(string.IsNullOrEmpty((string)obj.Token));
        }

        [Fact]
        public async Task Login_WhenCredentialsAreInvalid_ReturnsUnauthorized()
        {
            // Arrange
            var model = new BrokerLogin { Username = faker.Internet.UserName(), Password = faker.Internet.Password() };
            userManagerMock.Setup(x => x.FindByNameAsync(model.Username)).ReturnsAsync(new IdentityUser());
            userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<IdentityUser>(), model.Password)).ReturnsAsync(false);

            // Act
            var result = await accountController.LoginAsync(model);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }
    }
}
