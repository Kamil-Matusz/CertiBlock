using CertiBlock.Shared.Exceptions;

namespace CertiBlock.Services.Certificates.Core.Exceptions;

public class CertificateNotFoundException(Guid id) : CustomException($"Certificate with ID: '{id}' was not found.")
{
    public Guid Id { get; set; } = id;
}