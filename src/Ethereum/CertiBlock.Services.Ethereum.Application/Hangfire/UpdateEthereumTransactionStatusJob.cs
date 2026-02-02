using CertiBlock.Services.Ethereum.Application.Services.Ethereum;
using CertiBlock.Shared.Enums;
using Microsoft.Extensions.Logging;

namespace CertiBlock.Services.Ethereum.Application.Hangfire;

public class UpdateEthereumTransactionStatusJob(IEthereumService ethereumService, ILogger<UpdateEthereumTransactionStatusJob> logger)
{
    public async Task UpdateSubmittedTransactionsAsync()
    {
        var submittedTransactions = await ethereumService.GetEthereumTransactionsByStatusAsync(Status.Submitted);
        var transactionsList = submittedTransactions.ToList();
        
        foreach (var transaction in transactionsList)
        {
            try
            {
                var status = await ethereumService.GetEthereumTransactionStatusAsync(transaction.TransactionHash);

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