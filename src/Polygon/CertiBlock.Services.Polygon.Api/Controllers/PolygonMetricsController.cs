using CertiBlock.Services.Polygon.Application.Services.PolygonMetrics;
using CertiBlock.Services.Polygon.Core.DTO;
using CertiBlock.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace CertiBlock.Services.Polygon.Api.Controllers;

public class PolygonMetricsController(IPolygonMetricService polygonMetricService) : BaseController
{
    [HttpPost("collectMetricsForPolygon")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CollectMetrics([FromBody] MetricDto dto)
        => Ok(await polygonMetricService.CollectMetricsAsync(dto.CertificateId, dto.TransactionHash));
    
    [HttpDelete("deleteTransactionMetricById/{certificateId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePolygonTransactionByCertificateId(Guid certificateId)
    {
        await polygonMetricService.DeleteTransactionMetricsByCertificateIdAsync(certificateId);
        return NoContent();
    }
    
    [HttpGet("getPolygonTransactionMetricsByCertificateId/{certificateId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PolygonMetricDetailsDto>> GetPolygonTransactionsById(Guid certificateId)
        => Ok(await polygonMetricService.GetTransactionMetricsByCertificateIdAsync(certificateId));
    
    [HttpGet("getPolygonResearchMetrics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<PolygonMetricResearchDto>>> GetResearchMetrics()
        => Ok(await polygonMetricService.GetAllResearchMetricsAsync());
}