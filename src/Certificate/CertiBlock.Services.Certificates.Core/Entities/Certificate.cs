using CertiBlock.Shared.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CertiBlock.Services.Certificates.Core.Entities;

public class Certificate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string OwnerName { get; set; }
    public string Title { get; set; }
    public string IssuedBy { get; set; }
    public DateTime IssuedDate { get; set; }
    public string CertificateHash { get; set; }
    public Blockchain Blockchain { get; set; }
    public string IssuerId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}