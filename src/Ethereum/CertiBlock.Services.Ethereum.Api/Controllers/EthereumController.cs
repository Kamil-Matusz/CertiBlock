using CertiBlock.Services.Ethereum.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CertiBlock.Services.Ethereum.Api.Controllers;

public class EthereumController(IEthereumService ethereumService) : BaseController
{
    [HttpGet("balance/{address}")]
    public async Task<IActionResult> GetBalance(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            return BadRequest("Address cannot be empty");

        try
        {
            var balance = await ethereumService.GetEthBalanceAsync(address);
            return Ok(new { Address = address, Balance = balance, Unit = "ETH" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Error while fetching balance", Details = ex.Message });
        }
    }
}