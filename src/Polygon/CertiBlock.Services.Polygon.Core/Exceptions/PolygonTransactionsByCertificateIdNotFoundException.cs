using CertiBlock.Shared.Exceptions;

namespace CertiBlock.Services.Polygon.Core.Exceptions;

public class PolygonTransactionsByCertificateIdNotFoundException(Guid certificateId)
    : CustomException($"Polygon transaction with ID: '{certificateId}' was not found.")
{
    public Guid? CertificateId { get; set; } = certificateId;
}