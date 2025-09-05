
using CertiBlock.Services.Certificates.Core.Clients.Ethereum;
using CertiBlock.Services.Certificates.Core.Clients.Polygon;
using CertiBlock.Services.Certificates.Core.DAL.Repositories;
using CertiBlock.Services.Certificates.Core.Entities;
using CertiBlock.Services.Certificates.Core.Exceptions;
using CertiBlock.Services.Certificates.Core.Services;
using CertiBlock.Shared.Enums;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace CertiBlock.Services.Certificates.UnitTests.Services;

public class CertificateServiceTests
{
    private readonly Mock<ICertificateRepository> _repositoryMock = new();
    private readonly Mock<IBus> _busMock = new();
    private readonly Mock<ILogger<CertificateService>> _loggerMock = new();
    private readonly Mock<IEthereumClient> _ethereumClientMock = new();
    private readonly Mock<IPolygonClient> _polygonClientMock = new();
    private readonly CertificateService _service;

    public CertificateServiceTests()
    {
        _service = new CertificateService(
            _repositoryMock.Object, 
            _busMock.Object, 
            _loggerMock.Object, 
            _ethereumClientMock.Object, 
            _polygonClientMock.Object);
    }
    
    [Fact]
    public async Task DeleteCertificateAsync_ShouldCallDelete_WhenCertificateExists()
    {
        // Arrange
        var certId = Guid.NewGuid();
        _repositoryMock
            .Setup(x => x.GetCertificateByIdAsync(certId))
            .ReturnsAsync(new Certificate { Id = certId });

        // Act
        await _service.DeleteCertificateAsync(certId);

        // Assert
        _repositoryMock.Verify(x => x.DeleteCertificateAsync(certId), Times.Once);
    }

    [Fact]
    public async Task DeleteCertificateAsync_ShouldThrow_WhenCertificateNotFound()
    {
        // Arrange
        var certId = Guid.NewGuid();
        _repositoryMock
            .Setup(x => x.GetCertificateByIdAsync(certId))!
            .ReturnsAsync((Certificate?)null);

        // Act & Assert
        await Should.ThrowAsync<CertificateNotFoundException>(() => _service.DeleteCertificateAsync(certId));
    }
    
    [Fact]
    public async Task GetCertificateByIdAsync_ShouldReturnCertificate()
    {
        // Arrange
        var certId = Guid.NewGuid();
        var certificate = new Certificate
        {
            Id = certId,
            OwnerName = "Anna",
            Title = "Test",
            IssuedBy = "Org",
            CertificateHash = "hash",
            Blockchain = Blockchain.Ethereum,
            TransactionHash = "tx",
            CreatedAt = DateTime.UtcNow
        };

        _repositoryMock
            .Setup(x => x.GetCertificateByIdAsync(certId))
            .ReturnsAsync(certificate);

        // Act
        var result = await _service.GetCertificateByIdAsync(certId);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(certId);
        result.OwnerName.ShouldBe("Anna");
    }
    
    [Fact]
    public async Task GetCertificateByIdAsync_ShouldThrow_WhenNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock
            .Setup(x => x.GetCertificateByIdAsync(id))!
            .ReturnsAsync((Certificate?)null);

        // Act & Assert
        await Should.ThrowAsync<CertificateNotFoundException>(() => _service.GetCertificateByIdAsync(id));
    }
    
    [Fact]
    public async Task GetCertificatesByUserIdAsync_ShouldReturnCertificates()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var certificates = new List<Certificate>
        {
            new Certificate { Id = Guid.NewGuid(), IssuerId = userId.ToString() }
        };

        _repositoryMock
            .Setup(x => x.GetCertificateByIssuerIdAsync(userId.ToString()))
            .ReturnsAsync(certificates);


        // Act
        var result = await _service.GetCertificatesByUserIdAsync(userId);

        // Assert
        var certificateDtos = result.ToList();
        certificateDtos.ShouldNotBeEmpty();
        certificateDtos.ShouldAllBe(x => x.IssuerId == userId.ToString());
    }
    
    [Fact]
    public async Task GetCertificatesByUserIdAsync_ShouldThrow_WhenNoneFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _repositoryMock
            .Setup(x => x.GetCertificateByIssuerIdAsync(userId.ToString()))
            .ReturnsAsync(new List<Certificate>());

        // Act & Assert
        await Should.ThrowAsync<CetrtificateForUserNotFoundException>(() =>
            _service.GetCertificatesByUserIdAsync(userId));
    }
}