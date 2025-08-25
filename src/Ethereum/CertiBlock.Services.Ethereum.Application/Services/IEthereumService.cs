namespace CertiBlock.Services.Ethereum.Application.Services;

public interface IEthereumService
{
    Task DeleteEthereumTransactionAsync(Guid id);
}