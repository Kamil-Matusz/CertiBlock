using CertiBlock.Services.Certificates.Core.DTO;
using CertiBlock.Services.Certificates.Core.Entities;
using CertiBlock.Services.Certificates.Core.Mappers;
using CertiBlock.Shared.Enums;
using Shouldly;

namespace CertiBlock.Services.Certificates.UnitTests.Mappers;

public class CertificateMapperTests
{
    private Certificate CreateSampleCertificate()
    {
        return new Certificate
        {
            Id = Guid.NewGuid(),
            OwnerName = "Jan Kowalski",
            Title = "Certyfikat .NET",
            IssuedBy = "Microsoft",
            IssuedDate = new DateTime(2023, 10, 15),
            CertificateHash = "abc123hash",
            Blockchain = Blockchain.Ethereum,
            TransactionHash = "0x123456789",
            IssuerId = "SampleIssuerId",
            CreatedAt = DateTime.UtcNow
        };
    }
    
    [Fact]
    public void Map_ShouldMapAllProperties_WhenCertificateIsValid()
    {
        // Arrange
        var certificate = CreateSampleCertificate();

        // Act
        var result = CertificateMapper.Map<CertificateDto>(certificate);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(certificate.Id);
        result.OwnerName.ShouldBe(certificate.OwnerName);
        result.Title.ShouldBe(certificate.Title);
        result.IssuedBy.ShouldBe(certificate.IssuedBy);
        result.IssuedDate.ShouldBe(certificate.IssuedDate);
        result.CertificateHash.ShouldBe(certificate.CertificateHash);
        result.Blockchain.ShouldBe(certificate.Blockchain);
        result.TransactionHash.ShouldBe(certificate.TransactionHash);
        result.IssuerId.ShouldBe(certificate.IssuerId);
        result.CreatedAt.ShouldBe(certificate.CreatedAt);
    }
    
    [Fact]
    public void MapAll_ShouldMapAllCertificates_WhenCollectionIsNotEmpty()
    {
        // Arrange
        var certificates = new List<Certificate>
        {
            CreateSampleCertificate(),
            CreateSampleCertificate(),
            CreateSampleCertificate()
        };

        // Act
        var result = CertificateMapper.MapAll<CertificateDto>(certificates).ToList();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(3);
        result.All(dto => dto is not null).ShouldBeTrue();
        result.All(dto => dto is CertificateDto).ShouldBeTrue();
    }
    
    [Fact]
    public void MapAll_ShouldReturnEmptyCollection_WhenInputCollectionIsEmpty()
    {
        // Arrange
        var certificates = new List<Certificate>();

        // Act
        var result = CertificateMapper.MapAll<CertificateDto>(certificates);

        // Assert
        result.ShouldNotBeNull();
        result.Count().ShouldBe(0);
    }
    
    [Fact]
    public void MapAll_ShouldPreserveOrder_WhenMappingMultipleCertificates()
    {
        // Arrange
        var certificate1 = CreateSampleCertificate();
        certificate1.Title = "First certificate";
        var certificate2 = CreateSampleCertificate();
        certificate2.Title = "Second certificate";
        var certificate3 = CreateSampleCertificate();
        certificate3.Title = "Third certificate";

        var certificates = new List<Certificate> { certificate1, certificate2, certificate3 };

        // Act
        var result = CertificateMapper.MapAll<CertificateDto>(certificates).ToList();

        // Assert
        result[0].Title.ShouldBe("First certificate");
        result[1].Title.ShouldBe("Second certificate");
        result[2].Title.ShouldBe("Third certificate");
    }

}