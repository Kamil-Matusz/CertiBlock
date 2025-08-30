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
    {
        var balanceDto = await ethereumService.GetEthBalanceAsync(walletAddress);
        return Ok(balanceDto);
    }

    [HttpPost("registerCertificate")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BlockchainTransaction>> RegisterCertificate([FromBody] BlockchainTransactionDto dto)
    {
        var transaction = await ethereumService.RegisterEthereumTransactionAsync(dto);
        return Ok(transaction);
    }
}