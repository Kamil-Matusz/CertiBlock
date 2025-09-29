using CertiBlock.Services.Ethereum.Application.Services.EthereumMetrics;
using CertiBlock.Services.Ethereum.Core.DTO;
using Microsoft.AspNetCore.Mvc;

namespace CertiBlock.Services.Ethereum.Api.Controllers;

public class EthereumMetricsController(IEthereumMetricService ethereumMetricService) : BaseController
{
    [HttpPost("collectMetricsForEthereum")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async  Task<IActionResult> CollectMetrics([FromBody] EthereumMetricDto dto)
        => Ok(await ethereumMetricService.CollectMetricsAsync(dto.CertificateId, dto.TransactionHash));
    
    [HttpDelete("deleteTransactionMetricById/{certificateId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async  Task<IActionResult> DeleteTransactionByCertificateId(Guid certificateId)
    {
        await ethereumMetricService.DeleteTransactionMetricsByCertificateIdAsync(certificateId);
        return NoContent();
    }
    
    [HttpGet("getEthereumTransactionMetricsByCertificateId/{certificateId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EthereumMetricDetailsDto>> GetEthereumTransactionsById(Guid certificateId)
        => Ok(await ethereumMetricService.GetTransactionMetricsByCertificateIdAsync(certificateId));
}