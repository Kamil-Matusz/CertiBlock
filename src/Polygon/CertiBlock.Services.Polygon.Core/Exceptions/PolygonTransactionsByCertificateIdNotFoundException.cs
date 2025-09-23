using CertiBlock.Shared.Exceptions;

namespace CertiBlock.Services.Polygon.Core.Exceptions;

public class PolygonTransactionsByCertificateIdNotFoundException : CustomException
{
    public Guid? CertificateId { get; set; }

    public PolygonTransactionsByCertificateIdNotFoundException(Guid certificateId) 
        : base($"Polygon transaction with ID: '{certificateId}' was not found.")
    {
        CertificateId = certificateId;
    }
}