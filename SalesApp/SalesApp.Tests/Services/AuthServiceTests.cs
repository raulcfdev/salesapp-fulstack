using Microsoft.AspNetCore.Identity;
using Moq;
using SalesApp.DTOs.AuthDTOs;
using SalesApp.Models;
using SalesApp.Services;
using SalesApp.Services.Interfaces;
using System.Threading.Tasks;
using Xunit;

namespace SalesApp.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<SignInManager<User>> _mockSignInManager;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            var contextAccessorMock = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var userPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<User>>();
            _mockSignInManager = new Mock<SignInManager<User>>(
                _mockUserManager.Object, contextAccessorMock.Object, userPrincipalFactoryMock.Object, null, null, null, null);

            _mockTokenService = new Mock<ITokenService>();

            _authService = new AuthService(
                _mockUserManager.Object,
                _mockSignInManager.Object,
                _mockTokenService.Object);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsAuthResponse()
        {
            var loginDto = new LoginDTO { Email = "test@example.com", Password = "Password123!" };
            var user = new User { Id = "user1", Email = "test@example.com", Name = "Test User" };
            var signInResult = SignInResult.Success;
            var token = "test-jwt-token";

            _mockUserManager.Setup(x => x.FindByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);
            _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(user, loginDto.Password, false))
                .ReturnsAsync(signInResult);
            _mockTokenService.Setup(x => x.GenerateToken(user))
                .Returns(token);

            var result = await _authService.LoginAsync(loginDto);

            Assert.NotNull(result);
            Assert.Equal(token, result.Token);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.Name, result.Name);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidEmail_ReturnsNull()
        {
            var loginDto = new LoginDTO { Email = "nonexistent@example.com", Password = "Password123!" };

            _mockUserManager.Setup(x => x.FindByEmailAsync(loginDto.Email))
                .ReturnsAsync((User)null);

            var result = await _authService.LoginAsync(loginDto);

            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidPassword_ReturnsNull()
        {
            var loginDto = new LoginDTO { Email = "test@example.com", Password = "WrongPassword!" };
            var user = new User { Id = "user1", Email = "test@example.com", Name = "Test User" };
            var signInResult = SignInResult.Failed;

            _mockUserManager.Setup(x => x.FindByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);
            _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(user, loginDto.Password, false))
                .ReturnsAsync(signInResult);

            var result = await _authService.LoginAsync(loginDto);

            Assert.Null(result);
        }

        [Fact]
        public async Task RegisterAsync_WithValidData_ReturnsAuthResponse()
        {
            var registerDto = new RegisterDTO
            {
                Email = "newuser@example.com",
                Password = "Password123!",
                Name = "New User"
            };
            var identityResult = IdentityResult.Success;
            var token = "new-user-jwt-token";

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), registerDto.Password))
                .ReturnsAsync(identityResult)
                .Callback<User, string>((user, password) =>
                {
                    user.Id = "new-user-id";
                });

            _mockTokenService.Setup(x => x.GenerateToken(It.IsAny<User>()))
                .Returns(token);

            var result = await _authService.RegisterAsync(registerDto);

            Assert.NotNull(result);
            Assert.Equal(token, result.Token);
            Assert.Equal(registerDto.Email, result.Email);
            Assert.Equal(registerDto.Name, result.Name);

            _mockUserManager.Verify(x => x.CreateAsync(
                It.Is<User>(u =>
                    u.Email == registerDto.Email &&
                    u.UserName == registerDto.Email &&
                    u.Name == registerDto.Name),
                registerDto.Password),
                Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_WhenCreateFails_ReturnsNull()
        {
            var registerDto = new RegisterDTO
            {
                Email = "newuser@example.com",
                Password = "Password123!",
                Name = "New User"
            };
            var identityResult = IdentityResult.Failed(new IdentityError { Description = "Error" });

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), registerDto.Password))
                .ReturnsAsync(identityResult);

            var result = await _authService.RegisterAsync(registerDto);

            Assert.Null(result);
        }
    }
}