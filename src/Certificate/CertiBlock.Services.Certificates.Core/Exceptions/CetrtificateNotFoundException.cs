using CertiBlock.Shared.Exceptions;

namespace CertiBlock.Services.Certificates.Core.Exceptions;

public class CetrtificateNotFoundException : CustomException
{
    public Guid Id { get; set; }
    
    public CetrtificateNotFoundException(Guid id) : base($"Certificate with ID: '{id}' was not found.")
    {
        Id = id;
    }
}