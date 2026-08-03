using System.Net;
using CertiBlock.Shared.Exceptions;

namespace CertiBlock.Services.Certificates.Core.Exceptions;

internal sealed class CertificateExceptionMapper : IExceptionMapper
{
    public ExceptionResponse? Map(Exception exception)
        => exception switch
        {
            CertificateNotFoundException ex =>
                new ExceptionResponse(new { code = "certificate_not_found", message = ex.Message }, HttpStatusCode.NotFound),

            CertificateForUserNotFoundException ex =>
                new ExceptionResponse(new { code = "certificate_for_user_not_found", message = ex.Message }, HttpStatusCode.NotFound),

            UnsupportedBlockchainException ex =>
                new ExceptionResponse(new { code = "unsupported_blockchain", message = ex.Message }, HttpStatusCode.BadRequest),

            BlockchainTransactionFailedException ex =>
                new ExceptionResponse(new { code = "blockchain_transaction_failed", message = ex.Message }, HttpStatusCode.UnprocessableEntity),

            BlockchainConfigurationException ex =>
                new ExceptionResponse(new { code = "blockchain_configuration_error", message = ex.Message }, HttpStatusCode.InternalServerError),

            _ => null
        };
}