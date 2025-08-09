using CertiBlock.Services.Certificates.Core.Clients;
using CertiBlock.Services.Certificates.Core.DTO;
using CertiBlock.Shared.Enums;
using Shouldly;

namespace CertiBlock.Services.Certificates.UnitTests.Clients;

public class BlockchainRouterClientTests
{
    [Fact]
    public async Task SendToBlockchainAsync_ShouldReturnMockedTransactionHash()
    {
        // Arrange
        var httpClient = new HttpClient();
        var client = new BlockchainRouterClient(httpClient);
        var request = new BlockchainRegisterRequest
        {
            Hash = "CERT123",
            Issuer = "Kamil Matusz",
            Blockchain = Blockchain.Ethereum
        };

        // Act
        var result = await client.SendToBlockchainAsync(request);

        // Assert
        result.ShouldBe("0xMOCKED_TRANSACTION_HASH");
    }
}