using CertiBlock.Shared.Exceptions;

namespace CertiBlock.Services.Certificates.Core.Exceptions;

public class CertificateForUserNotFoundException(Guid id) : CustomException($"Certificate for User with ID: '{id}' was not found.")
{
    public Guid Id { get; set; } = id;
}