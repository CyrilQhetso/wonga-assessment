using AuthService.Models;
using AuthService.Services;
using AuthService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using FluentAssertions;
using Moq;

namespace AuthService.Tests;

public class AuthServiceTests
{
    private AppDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private IConfiguration CreateConfig() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt__Secret", "test-secret-key-that-is-long-enough-32chars" },
                { "Jwt__Issuer", "TestIssuer" },
                { "Jwt__Audience", "TestAudience" },
                { "Jwt__ExpiryMinutes", "60" }
            })
            .Build();

    [Fact]
    public async Task Register_WithValidData_ReturnsAuthResponse()
    {
        var db = CreateInMemoryDb();
        var svc = new AuthenticationService(db, CreateConfig());

        var result = await svc.RegisterAsync(new RegisterRequest(
            "Mike", "james", "mike@james.com", "Password123!"));

        result.Should().NotBeNull();
        result!.User.Email.Should().Be("mike@james.com");
        result.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsNull()
    {
        var db = CreateInMemoryDb();
        var svc = new AuthenticationService(db, CreateConfig());

        await svc.RegisterAsync(new RegisterRequest("Mike", "james", "mike@james.com", "Pass123!"));
        var result = await svc.RegisterAsync(new RegisterRequest("James", "Mike", "mike@james.com", "Pass123!"));

        result.Should().BeNull();
    }

    [Fact]
    public async Task Login_WithCorrectCredentials_ReturnsAuthResponse()
    {
        var db = CreateInMemoryDb();
        var svc = new AuthenticationService(db, CreateConfig());

        await svc.RegisterAsync(new RegisterRequest("Mike", "james", "mike@james.com", "Pass123!"));
        var result = await svc.LoginAsync(new LoginRequest("mike@james.com", "Pass123!"));

        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsNull()
    {
        var db = CreateInMemoryDb();
        var svc = new AuthenticationService(db, CreateConfig());

        await svc.RegisterAsync(new RegisterRequest("Mike", "james", "mike@james.com", "Pass123!"));
        var result = await svc.LoginAsync(new LoginRequest("mike@james.com", "WrongPass!"));

        result.Should().BeNull();
    }

    [Fact]
    public async Task Login_EmailIsCaseInsensitive()
    {
        var db = CreateInMemoryDb();
        var svc = new AuthenticationService(db, CreateConfig());

        await svc.RegisterAsync(new RegisterRequest("Mike", "James", "MIKE@JAMES.COM", "Pass123!"));
        var result = await svc.LoginAsync(new LoginRequest("mike@james.com", "Pass123!"));

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetUserDetails_WithValidId_ReturnsUserDetails()
    {
        var db = CreateInMemoryDb();
        var svc = new AuthenticationService(db, CreateConfig());

        var registered = await svc.RegisterAsync(new RegisterRequest(
            "Mike", "James", "mike@james.com", "Pass123!"));

        var result = await svc.GetUserDetailsAsync(registered!.User.Id);

        result.Should().NotBeNull();
        result!.Email.Should().Be("mike@james.com");
        result.FirstName.Should().Be("Mike");
    }

    [Fact]
    public async Task GetUserDetails_WithInvalidId_ReturnsNull()
    {
        var db = CreateInMemoryDb();
        var svc = new AuthenticationService(db, CreateConfig());

        var result = await svc.GetUserDetailsAsync(Guid.NewGuid());

        result.Should().BeNull();
    }
}