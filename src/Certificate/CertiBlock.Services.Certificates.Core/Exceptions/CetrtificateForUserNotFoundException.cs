using CertiBlock.Shared.Exceptions;

namespace CertiBlock.Services.Certificates.Core.Exceptions;

public class CetrtificateForUserNotFoundException : CustomException
{
    public Guid Id { get; set; }
    
    public CetrtificateForUserNotFoundException(Guid id) : base($"Certificate for User with ID: '{id}' was not found.")
    {
        Id = id;
    }
}