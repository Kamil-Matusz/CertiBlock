using CertiBlock.Shared.Exceptions;

namespace CertiBlock.Services.Polygon.Core.Exceptions;

public class PolygonTransactionsNotFoundException : CustomException
{
    public Guid? Id { get; set; }
    public string? StringId { get; set; }
    public Guid? CertificateId { get; set; }

    public PolygonTransactionsNotFoundException(Guid id) 
        : base($"Polygon transaction with ID: '{id}' was not found.")
    {
        Id = id;
    }

    public PolygonTransactionsNotFoundException(string id) 
        : base($"Polygon transaction with ID: '{id}' was not found.")
    {
        StringId = id;
    }
}