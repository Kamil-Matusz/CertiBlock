namespace CertiBlock.Services.Ethereum.Application.Facade;

public interface IEthereumFacade
{
    Task DeleteEthereumTransactionWithMetricsAsync(Guid certificateId);
}