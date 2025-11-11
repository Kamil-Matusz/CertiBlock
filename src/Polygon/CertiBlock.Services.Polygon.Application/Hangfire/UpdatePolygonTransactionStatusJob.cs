using CertiBlock.Services.Polygon.Application.Services.Polygon;
using CertiBlock.Shared.Enums;
using Microsoft.Extensions.Logging;

namespace CertiBlock.Services.Polygon.Application.Hangfire;

public class UpdatePolygonTransactionStatusJob(IPolygonService polygonService, ILogger<UpdatePolygonTransactionStatusJob> logger)
{
    public async Task UpdateSubmittedTransactionsAsync()
    {
        var submittedTransactions = await polygonService.GetPolygonTransactionsByStatusAsync(Status.Submitted);
        var transactionsList = submittedTransactions.ToList();
        
        foreach (var transaction in transactionsList)
        {
            try
            {
                var status = await polygonService.GetPolygonTransactionStatusAsync(transaction.TransactionHash);

                logger.LogInformation(
                    "Transaction {TransactionHash} status updated to {Status} with {Confirmations} confirmations",
                    transaction.TransactionHash,
                    status.Status,
                    status.Confirmations);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating status for transaction {TransactionHash}", transaction.TransactionHash);
            }
        }
    }
}