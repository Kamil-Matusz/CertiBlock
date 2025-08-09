using CertiBlock.Shared.Exceptions;

namespace CertiBlock.Services.Certificates.Core.Exceptions;

public class CertificateNotFoundException : CustomException
{
    public Guid Id { get; set; }
    
    public CertificateNotFoundException(Guid id) : base($"Certificate with ID: '{id}' was not found.")
    {
        Id = id;
    }
}