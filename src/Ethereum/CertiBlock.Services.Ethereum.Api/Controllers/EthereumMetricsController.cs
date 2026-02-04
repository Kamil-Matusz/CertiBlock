using System.Text;
using System.Text.Json;
using CertiBlock.Services.Ethereum.Application.RabbitMQ;
using CertiBlock.Services.Ethereum.Application.Services.EthereumMetrics;
using CertiBlock.Services.Ethereum.Core.DTO;
using CertiBlock.Shared.DTO;
using CertiBlock.Shared.Enums;
using CertiBlock.Shared.Messaging;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

namespace CertiBlock.Services.Ethereum.Api.Controllers;

public class EthereumMetricsController(IEthereumMetricService ethereumMetricService, MetricPublisher metricPublisher) : BaseController
{
    [HttpPost("collectMetricsForEthereum")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CollectMetrics([FromBody] MetricDto dto)
        => Ok(await ethereumMetricService.CollectMetricsAsync(dto.CertificateId, dto.TransactionHash));
    
    [HttpDelete("deleteTransactionMetricById/{certificateId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTransactionByCertificateId(Guid certificateId)
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
    
    [HttpGet("research")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ResearchMetricDto>>> GetResearchMetrics()
        => Ok(await ethereumMetricService.GetAllResearchMetricsAsync());

    [HttpPost("test")]
    public IActionResult PublishTestMetric()
    {
        var metricEvent = new MetricCollectedEvent(
            Guid.NewGuid(),
            Blockchain.Ethereum,
            Operation.Register,
            21000,
            15.0,
            0.00042,
            null,
            DateTime.UtcNow
        );

        metricPublisher.Publish(metricEvent);

        return Ok(new { status = "sent", metricEvent });
    }
}