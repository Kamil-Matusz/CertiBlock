using System.Net;
using CertiBlock.Shared.Exceptions;

namespace CertiBlock.Services.Polygon.Core.Exceptions;

public sealed class PolygonExceptionMapper : IExceptionMapper
{
    public ExceptionResponse? Map(Exception exception)
        => exception switch
        {
            PolygonTransactionsNotFoundException ex =>
                new ExceptionResponse(new { code = "polygon_transaction_not_found", message = ex.Message }, HttpStatusCode.NotFound),

            PolygonTransactionsByCertificateIdNotFoundException ex =>
                new ExceptionResponse(new { code = "polygon_transaction_by_certificate_not_found", message = ex.Message }, HttpStatusCode.NotFound),

            PolygonTransactionsByHashNotFoundException ex =>
                new ExceptionResponse(new { code = "polygon_transaction_by_hash_not_found", message = ex.Message }, HttpStatusCode.NotFound),

            PolygonBalanceException ex =>
                new ExceptionResponse(new { code = "polygon_balance_error", message = ex.Message }, HttpStatusCode.BadGateway),

            _ => null
        };
}