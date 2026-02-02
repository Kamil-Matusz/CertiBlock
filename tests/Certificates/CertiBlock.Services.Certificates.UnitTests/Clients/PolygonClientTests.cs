using System.Net;
using System.Text.Json;
using CertiBlock.Services.Certificates.Core.Clients.Polygon;
using CertiBlock.Services.Certificates.Core.DTO;
using CertiBlock.Services.Certificates.Core.Events;
using CertiBlock.Shared.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Shouldly;

namespace CertiBlock.Services.Certificates.UnitTests.Clients;

public class PolygonClientTests
{
    private readonly Mock<ILogger<PolygonClient>> _loggerMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly PolygonClient _polygonClient;
    
    public PolygonClientTests()
    {
        _loggerMock = new Mock<ILogger<PolygonClient>>();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://test.api.com")
        };
        _polygonClient = new PolygonClient(_httpClient, _loggerMock.Object);
    }
    
    [Fact]
    public async Task RegisterCertificateAsync_WhenSuccessful_ShouldSendCorrectRequestAndLogSuccess()
    {
        // Arrange
        var certificate = new CertificateRegistered(Guid.NewGuid(), "Cert-123", "Kamil", Blockchain.Polygon);
        var expectedResponse =
            new CertificateRegistrationResponse(Guid.NewGuid(), "Cert-123", "Polygon", DateTime.Now);

        var responseJson = JsonSerializer.Serialize(expectedResponse);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, System.Text.Encoding.UTF8, "application/json")
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        await _polygonClient.RegisterCertificateAsync(certificate);

        // Assert
        _httpMessageHandlerMock
            .Protected()
            .Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri!.ToString().EndsWith("/polygon-service/Polygon/registerCertificate")),
                
                ItExpr.IsAny<CancellationToken>());

        // Verify logging
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString()!.StartsWith("[Polygon] Register certificate")
                ),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ), Times.Once);
    }
    
    [Fact]
    public async Task RegisterCertificateAsync_WhenHttpClientThrowsException_ShouldPropagateException()
    {
        // Arrange
        var certificateId = Guid.NewGuid();
        var certificate = new CertificateRegistered(certificateId, "hash-exception", "exception-issuer", Blockchain.Polygon);

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act & Assert
        var exception = await Should.ThrowAsync<HttpRequestException>(
            () => _polygonClient.RegisterCertificateAsync(certificate));

        exception.Message.ShouldBe("Network error");
    }
}