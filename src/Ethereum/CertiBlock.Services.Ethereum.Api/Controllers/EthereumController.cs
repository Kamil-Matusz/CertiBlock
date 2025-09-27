using CertiBlock.Services.Ethereum.Application.Services;
using CertiBlock.Services.Ethereum.Application.Services.Ethereum;
using CertiBlock.Services.Ethereum.Core.DTO;
using CertiBlock.Shared.Enums;
using Microsoft.AspNetCore.Mvc;

namespace CertiBlock.Services.Ethereum.Api.Controllers;

public class EthereumController(IEthereumService ethereumService) : BaseController
{
    [HttpGet("getEthBalanceByWalletAddress/{walletAddress}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EthereumBalanceDto>> GetEthereumBalanceByWalletAddress(string walletAddress)
        => Ok(await ethereumService.GetEthBalanceAsync(walletAddress));

    [HttpPost("registerCertificate")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BlockchainTransactionResultDto>> RegisterCertificate(
        [FromBody] BlockchainTransactionDto dto)
        => Ok(await ethereumService.RegisterEthereumTransactionAsync(dto));

    [HttpGet("getEthereumTransactionStatus/{transactionHash}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BlockchainTransactionStatusDto>> GetTransactionStatus(string transactionHash)
        => Ok(await ethereumService.GetEthereumTransactionStatusAsync(transactionHash));
    
    [HttpGet("getEthereumTransactionByStatus/{status}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<BlockchainTransactionDto>>> GetTransactionsByStatus(Status status)
        => Ok(await ethereumService.GetEthereumTransactionsByStatusAsync(status));
    
    [HttpGet("getEthereumTransactionByStatusFilter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<BlockchainTransactionDto>>> GetTransactionsByMultipleStatus([FromQuery] Status[] statuses)
        => Ok(await ethereumService.GetEthereumTransactionsByStatusAsync(statuses));
    
    [HttpGet("countEthereumTransactions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<long>> GetTransactionCount()
        => Ok(await ethereumService.GetEthereumTransactionCountAsync());
    
    [HttpGet("getEthereumTransactions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BlockchainTransactionResultDto>> GetEthereumTransactionsPaged([FromQuery] int pageIndex, [FromQuery] int pageSize)
        => Ok(await ethereumService.GetEthereumTransactionsPagedAsync(pageIndex, pageSize));
    
    [HttpGet("getEthereumAllTransactions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BlockchainTransactionResultDto>> GetAllEthereumTransactions()
        => Ok(await ethereumService.GetAllEthereumTransactionsAsync());

    [HttpDelete("deleteTransactionById/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<long>> DeleteTransactionById(Guid id)
    {
        await ethereumService.DeleteEthereumTransactionAsync(id);
        return NoContent();
    }
    
    [HttpGet("getEthereumTransactionById/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BlockchainTransactionDto>> GetEthereumTransactionsById(Guid id)
        => Ok(await ethereumService.GetEthereumTransactionByIdAsync(id));
    
    [HttpGet("getEthereumTransactionByCertificateId/{certificateId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BlockchainTransactionDto>> GetEthereumTransactionsByCertificateId(Guid certificateId)
        => Ok(await ethereumService.GetBlockchainTransactionByCertificateIdAsync(certificateId));
}