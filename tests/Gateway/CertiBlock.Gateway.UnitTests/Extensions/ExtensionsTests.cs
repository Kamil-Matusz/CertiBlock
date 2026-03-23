using CertiBlock.Gateway.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Yarp.ReverseProxy.Configuration;

namespace CertiBlock.Gateway.UnitTests.Extensions;

public class ExtensionsTests
{
    [Fact]
    public void AddGateway_ShouldRegisterAuthorizationPolicies()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Auth:Issuer", "test-issuer" },
                { "Auth:Audience", "test-audience" },
                { "Auth:SigningKey", "test-signing-key-that-is-long-enough" }
            }!)
            .Build();

        // Act
        services.AddGateway(configuration);
        var serviceProvider = services.BuildServiceProvider();
        var authService = serviceProvider.GetService<IAuthorizationPolicyProvider>();

        // Assert
        authService.ShouldNotBeNull();
        
        var requireAuthPolicy = authService.GetPolicyAsync("RequireAuth").Result;
        requireAuthPolicy.ShouldNotBeNull();
        requireAuthPolicy.Requirements.ShouldContain(r => r is DenyAnonymousAuthorizationRequirement);
    }

    [Fact]
    public void AddGateway_ShouldRegisterReverseProxy()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var configuration = BuildTestConfiguration();

        // Act
        services.AddGateway(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var proxyConfigProvider = serviceProvider.GetService<IProxyConfigProvider>();
        proxyConfigProvider.ShouldNotBeNull();
    }
    
    private static IConfiguration BuildTestConfiguration()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            {"Auth:Issuer", "test-issuer"},
            {"Auth:Audience", "test-audience"},
            {"Auth:SigningKey", "test-signing-key-that-is-long-enough"},
            {"ReverseProxy:Routes:test:ClusterId", "test-cluster"},
            {"ReverseProxy:Routes:test:Match:Path", "/test/{**catch-all}"},
            {"ReverseProxy:Clusters:test-cluster:Destinations:d1:Address", "http://localhost:5000"}
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();
    }
}