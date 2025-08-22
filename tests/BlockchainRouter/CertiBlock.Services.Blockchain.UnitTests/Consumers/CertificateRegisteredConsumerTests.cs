using CertiBlock.Services.Blockchain.Core.Clients.Ethereum;
using CertiBlock.Services.Blockchain.Core.Clients.Polygon;
using CertiBlock.Services.Blockchain.Core.Events;
using CertiBlock.Services.Blockchain.Core.Exceptions;
using CertiBlock.Services.Blockchain.Core.MassTransit.Consumers;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace CertiBlock.Services.Blockchain.UnitTests.Consumers;

public class CertificateRegisteredConsumerTests
{
    private readonly Mock<IEthereumClient> _ethereumClientMock;
    private readonly Mock<IPolygonClient> _polygonClientMock;
    private readonly Mock<ConsumeContext<CertificateRegistered>> _contextMock;
    private readonly CertificateRegisteredConsumer _consumer;

    public CertificateRegisteredConsumerTests()
    {
        var loggerMock = new Mock<ILogger<CertificateRegisteredConsumer>>();
        _ethereumClientMock = new Mock<IEthereumClient>();
        _polygonClientMock = new Mock<IPolygonClient>();
        _contextMock = new Mock<ConsumeContext<CertificateRegistered>>();
            
        _consumer = new CertificateRegisteredConsumer(
            loggerMock.Object,
            _ethereumClientMock.Object,
            _polygonClientMock.Object);
    }
    
    [Fact]
    public async Task Consume_WhenBlockchainIsEthereum_ShouldCallEthereumClient()
    {
        // Arrange
        var certificate = new CertificateRegistered(
            Guid.NewGuid(),
            "test-hash",
            "test-issuer",
            Shared.Enums.Blockchain.Ethereum
        );

        _contextMock.Setup(x => x.Message).Returns(certificate);
        _ethereumClientMock.Setup(x => x.RegisterCertificateAsync(certificate))
            .Returns(Task.CompletedTask);

        // Act
        await _consumer.Consume(_contextMock.Object);

        // Assert
        _ethereumClientMock.Verify(x => x.RegisterCertificateAsync(certificate), Times.Once);
        _polygonClientMock.Verify(x => x.RegisterCertificateAsync(It.IsAny<CertificateRegistered>()), Times.Never);
    }
    
    [Fact]
    public async Task Consume_WhenBlockchainIsPolygon_ShouldCallPolygonClient()
    {
        // Arrange
        var certificate = new CertificateRegistered(
            Guid.NewGuid(),
            "test-hash",
            "test-issuer",
            Shared.Enums.Blockchain.Polygon
        );

        _contextMock.Setup(x => x.Message).Returns(certificate);
        _polygonClientMock.Setup(x => x.RegisterCertificateAsync(certificate))
            .Returns(Task.CompletedTask);

        // Act
        await _consumer.Consume(_contextMock.Object);

        // Assert
        _polygonClientMock.Verify(x => x.RegisterCertificateAsync(certificate), Times.Once);
        _ethereumClientMock.Verify(x => x.RegisterCertificateAsync(It.IsAny<CertificateRegistered>()), Times.Never);
    }
    
    [Fact]
    public async Task Consume_WhenUnsupportedBlockchain_ShouldThrowUnsupportedBlockchainException()
    {
        // Arrange
        var certificate = new CertificateRegistered(
            Guid.NewGuid(),
            "test-hash",
            "test-issuer",
            (Shared.Enums.Blockchain)999
        );

        _contextMock.Setup(x => x.Message).Returns(certificate);

        // Act & Assert
        var exception = await Should.ThrowAsync<UnsupportedBlockchainException>(
            () => _consumer.Consume(_contextMock.Object));

        exception.ShouldNotBeNull();
        _ethereumClientMock.Verify(x => x.RegisterCertificateAsync(It.IsAny<CertificateRegistered>()), Times.Never);
        _polygonClientMock.Verify(x => x.RegisterCertificateAsync(It.IsAny<CertificateRegistered>()), Times.Never);
    }
}