using CertiBlock.Services.Metrics.Core.Services;
using CertiBlock.Services.Metrics.Core.Services.Research;
using CertiBlock.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace CertiBlock.Services.Metrics.Api.Controllers;

public class ResearchController(IResearchService researchService) : BaseController
{
    [HttpGet("getComparativeResearchMetrics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ResearchMetricDto>>> GetComparativeResearchMetrics()
        => Ok(await researchService.GetComparativeResearchMetricsAsync());
}
