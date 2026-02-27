using AuthService.Data;
using AuthService.Models;
using AuthService.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace AuthService.Tests;

public class IntegrationTests
{
    private AppDbContext CreateDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("Integration_" + Guid.NewGuid())
            .Options);

    private IConfiguration CreateConfig() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt__Secret", "integration-test-secret-key-32chars!!" },
                { "Jwt__Issuer", "WongaAuthAPI" },
                { "Jwt__Audience", "WongaClient" },
                { "Jwt__ExpiryMinutes", "60" }
            })
            .Build();

    [Fact]
    public async Task FullFlow_RegisterThenLogin_ReturnsSameUser()
    {
        var db = CreateDb();
        var svc = new AuthenticationService(db, CreateConfig());

        var registered = await svc.RegisterAsync(new RegisterRequest(
            "Jane", "Smith", "jane@smith.com", "Password123!"));

        var loggedIn = await svc.LoginAsync(new LoginRequest(
            "jane@smith.com", "Password123!"));

        registered.Should().NotBeNull();
        loggedIn.Should().NotBeNull();
        loggedIn!.User.Email.Should().Be(registered!.User.Email);
        loggedIn.User.Id.Should().Be(registered.User.Id);
        loggedIn.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task FullFlow_RegisterThenGetDetails_ReturnsCorrectUser()
    {
        var db = CreateDb();
        var svc = new AuthenticationService(db, CreateConfig());

        var registered = await svc.RegisterAsync(new RegisterRequest(
            "Mike", "Jones", "mike@jones.com", "Password123!"));

        var details = await svc.GetUserDetailsAsync(registered!.User.Id);

        details.Should().NotBeNull();
        details!.FirstName.Should().Be("Mike");
        details.LastName.Should().Be("Jones");
        details.Email.Should().Be("mike@jones.com");
    }

    [Fact]
    public async Task FullFlow_PasswordIsHashed_NotStoredAsPlainText()
    {
        var db = CreateDb();
        var svc = new AuthenticationService(db, CreateConfig());

        await svc.RegisterAsync(new RegisterRequest(
            "Jane", "Smith", "jane2@smith.com", "Password123!"));

        var userInDb = db.Users.First();

        userInDb.PasswordHash.Should().NotBe("Password123!");
        userInDb.PasswordHash.Should().StartWith("$2");
    }

    [Fact]
    public async Task FullFlow_TwoUsers_DataIsIsolated()
    {
        var db = CreateDb();
        var svc = new AuthenticationService(db, CreateConfig());

        var user1 = await svc.RegisterAsync(new RegisterRequest(
            "User", "One", "user1@example.com", "Password123!"));

        var user2 = await svc.RegisterAsync(new RegisterRequest(
            "User", "Two", "user2@example.com", "Password123!"));

        user1.Should().NotBeNull();
        user2.Should().NotBeNull();
        user1!.User.Id.Should().NotBe(user2!.User.Id);
        db.Users.Count().Should().Be(2);
    }

    [Fact]
    public async Task FullFlow_LoginWithWrongPassword_DoesNotReturnToken()
    {
        var db = CreateDb();
        var svc = new AuthenticationService(db, CreateConfig());

        await svc.RegisterAsync(new RegisterRequest(
            "Jane", "Smith", "jane3@smith.com", "CorrectPassword!"));

        var result = await svc.LoginAsync(new LoginRequest(
            "jane3@smith.com", "WrongPassword!"));

        result.Should().BeNull();
    }

    [Fact]
    public async Task FullFlow_DuplicateEmail_OnlyOneUserCreated()
    {
        var db = CreateDb();
        var svc = new AuthenticationService(db, CreateConfig());

        await svc.RegisterAsync(new RegisterRequest(
            "Jane", "Smith", "jane@smith.com", "Password123!"));

        await svc.RegisterAsync(new RegisterRequest(
            "John", "Doe", "jane@smith.com", "Password123!"));

        db.Users.Count().Should().Be(1);
    }
}