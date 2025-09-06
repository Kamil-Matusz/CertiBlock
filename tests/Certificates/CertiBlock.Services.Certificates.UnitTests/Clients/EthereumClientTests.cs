using System.Net;
using System.Text.Json;
using CertiBlock.Services.Certificates.Core.Clients.Ethereum;
using CertiBlock.Services.Certificates.Core.DTO;
using CertiBlock.Services.Certificates.Core.Events;
using CertiBlock.Shared.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Shouldly;

namespace CertiBlock.Services.Certificates.UnitTests.Clients;

public class EthereumClientTests
{
    private readonly Mock<ILogger<EthereumClient>> _loggerMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly EthereumClient _sut;

    public EthereumClientTests()
    {
        _loggerMock = new Mock<ILogger<EthereumClient>>();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://test.api.com")
        };
        _sut = new EthereumClient(_httpClient, _loggerMock.Object);
    }
    
    [Fact]
    public async Task RegisterCertificateAsync_WhenSuccessful_ShouldSendCorrectRequestAndLogSuccess()
    {
        // Arrange
        var certificate = new CertificateRegistered(Guid.NewGuid(), "Cert-123", "Kamil", Blockchain.Ethereum);
        var expectedResponse =
            new CertificateRegistrationResponse(Guid.NewGuid(), "Cert-123", "Ethereum", DateTime.Now);

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
        await _sut.RegisterCertificateAsync(certificate);

        // Assert
        _httpMessageHandlerMock
            .Protected()
            .Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri!.ToString().EndsWith("/ethereum-service/Ethereum/registerCertificate")),
                
                ItExpr.IsAny<CancellationToken>());

        // Verify logging
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.StartsWith("[Ethereum] Zarejestrowano certyfikat")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
    
    [Fact]
    public async Task RegisterCertificateAsync_WhenHttpClientThrowsException_ShouldPropagateException()
    {
        // Arrange
        var certificateId = Guid.NewGuid();
        var certificate = new CertificateRegistered(certificateId, "hash-exception", "exception-issuer", Blockchain.Ethereum);

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act & Assert
        var exception = await Should.ThrowAsync<HttpRequestException>(
            () => _sut.RegisterCertificateAsync(certificate));

        exception.Message.ShouldBe("Network error");
    }
}