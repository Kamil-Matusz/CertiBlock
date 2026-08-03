using System.Net;
using CertiBlock.Shared.Exceptions;

namespace CertiBlock.Services.Ethereum.Core.Exceptions;

public sealed class EthereumExceptionMapper : IExceptionMapper
{
    public ExceptionResponse? Map(Exception exception)
        => exception switch
        {
            EthereumTransactionsNotFoundException ex =>
                new ExceptionResponse(new { code = "ethereum_transaction_not_found", message = ex.Message }, HttpStatusCode.NotFound),

            EthereumTransactionsByCertificateIdNotFoundException ex =>
                new ExceptionResponse(new { code = "ethereum_transaction_by_certificate_not_found", message = ex.Message }, HttpStatusCode.NotFound),

            EthereumTransactionsByHashNotFoundException ex =>
                new ExceptionResponse(new { code = "ethereum_transaction_by_hash_not_found", message = ex.Message }, HttpStatusCode.NotFound),

            EthereumBalanceException ex =>
                new ExceptionResponse(new { code = "ethereum_balance_error", message = ex.Message }, HttpStatusCode.BadGateway),

            _ => null
        };
}