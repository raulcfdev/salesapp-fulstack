using Microsoft.Extensions.Configuration;
using Moq;
using SalesApp.Models;
using SalesApp.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Xunit;

namespace SalesApp.Tests.Services
{
    public class TokenServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly TokenService _tokenService;
        private readonly string _testSecret = "ThisIsATestSecretKeyThatIsLongEnoughForHS256";

        public TokenServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            var mockConfigSection = new Mock<IConfigurationSection>();

            mockConfigSection.Setup(x => x.Value).Returns(_testSecret);
            _mockConfiguration.Setup(x => x.GetSection("JWT:Secret")).Returns(mockConfigSection.Object);
            _mockConfiguration.Setup(x => x["JWT:Secret"]).Returns(_testSecret);

            _tokenService = new TokenService(_mockConfiguration.Object);
        }

        [Fact]
        public void GenerateToken_ReturnsValidJwtToken()
        {
            var user = new User
            {
                Id = "user123",
                Email = "test@example.com",
                Name = "Test User"
            };

            var token = _tokenService.GenerateToken(user);

            Assert.NotEmpty(token);

            var tokenHandler = new JwtSecurityTokenHandler();
            Assert.True(tokenHandler.CanReadToken(token));

            var jwtToken = tokenHandler.ReadJwtToken(token);
            var claims = jwtToken.Claims.ToList();

            Assert.Contains(claims, c => c.Value == user.Id);
            Assert.Contains(claims, c => c.Value == user.Email);
            Assert.Contains(claims, c => c.Value == user.Name);
        }

        [Fact]
        public void GenerateToken_WithNullName_UsesEmptyString()
        {
            var user = new User
            {
                Id = "user123",
                Email = "test@example.com",
                Name = null
            };

            var token = _tokenService.GenerateToken(user);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var claims = jwtToken.Claims.ToList();

            Assert.Contains(claims, c => c.Value == string.Empty);
        }

        [Fact]
        public void GenerateToken_TokenHasCorrectExpiryTime()
        {
            var user = new User
            {
                Id = "user123",
                Email = "test@example.com",
                Name = "Test User"
            };

            var token = _tokenService.GenerateToken(user);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var expectedExpiration = DateTime.UtcNow.AddHours(8);
            var tokenExpiration = jwtToken.ValidTo;

            Assert.True(Math.Abs((expectedExpiration - tokenExpiration).TotalMinutes) < 1);
        }
    }
}