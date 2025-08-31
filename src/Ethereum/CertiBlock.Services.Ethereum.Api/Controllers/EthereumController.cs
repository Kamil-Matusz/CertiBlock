using CertiBlock.Services.Ethereum.Application.Services;
using CertiBlock.Services.Ethereum.Core.DTO;
using CertiBlock.Services.Ethereum.Core.Entities;
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

}