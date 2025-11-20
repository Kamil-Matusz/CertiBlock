using CertiBlock.Services.Polygon.Application.Facade;
using CertiBlock.Services.Polygon.Application.Services;
using CertiBlock.Services.Polygon.Application.Services.Polygon;
using CertiBlock.Services.Polygon.Core.DTO;
using CertiBlock.Shared.Enums;
using Microsoft.AspNetCore.Mvc;

namespace CertiBlock.Services.Polygon.Api.Controllers;

public class PolygonController(IPolygonService polygonService, IPolygonFacade polygonFacade) : BaseController
{
    [HttpGet("getMaticBalanceByWalletAddress/{walletAddress}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PolygonBalanceDto>> GetPolygonBalanceByWalletAddress(string walletAddress)
        => Ok(await polygonService.GetPolygonBalanceAsync(walletAddress));

    [HttpPost("registerCertificate")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BlockchainTransactionResultDto>> RegisterCertificate(
        [FromBody] BlockchainTransactionDto dto)
        => Ok(await polygonService.RegisterPolygonTransactionAsync(dto));

    [HttpGet("getPolygonTransactionStatus/{transactionHash}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BlockchainTransactionStatusDto>> GetTransactionStatus(string transactionHash)
        => Ok(await polygonService.GetPolygonTransactionStatusAsync(transactionHash));
    
    [HttpGet("getPolygonTransactionByStatus/{status}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<BlockchainTransactionDto>>> GetTransactionsByStatus(Status status)
        => Ok(await polygonService.GetPolygonTransactionsByStatusAsync(status));
    
    [HttpGet("getPolygonTransactionByStatusFilter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<BlockchainTransactionDto>>> GetTransactionsByMultipleStatus([FromQuery] Status[] statuses)
        => Ok(await polygonService.GetPolygonTransactionsByStatusAsync(statuses));
    
    [HttpGet("countPolygonTransactions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<long>> GetTransactionCount()
        => Ok(await polygonService.GetPolygonTransactionCountAsync());
    
    [HttpGet("getPolygonTransactions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BlockchainTransactionResultDto>> GetPolygonTransactionsPaged([FromQuery] int pageIndex, [FromQuery] int pageSize)
        => Ok(await polygonService.GetPolygonTransactionsPagedAsync(pageIndex, pageSize));
    
    [HttpGet("getPolygonAllTransactions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BlockchainTransactionResultDto>> GetAllPolygonTransactions()
        => Ok(await polygonService.GetAllPolygonTransactionsAsync());

    [HttpDelete("deleteTransactionById/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<long>> DeleteTransactionById(Guid id)
    {
        await polygonService.DeletePolygonTransactionAsync(id);
        return NoContent();
    }
    
    [HttpGet("getPolygonTransactionById/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BlockchainTransactionDto>> GetPolygonTransactionsById(Guid id)
        => Ok(await polygonService.GetPolygonTransactionByIdAsync(id));
    
    [HttpGet("getPolygonTransactionByCertificateId/{certificateId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BlockchainTransactionDto>> GetPolygonTransactionsByCertificateId(Guid certificateId)
        => Ok(await polygonService.GetBlockchainTransactionByCertificateIdAsync(certificateId));
    
    [HttpDelete("deleteTransactionByCertificateId/{certificateId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePolygonTransactionCertificateById(Guid certificateId)
    {
        await polygonFacade.DeletePolygonTransactionWithMetricsAsync(certificateId);
        return NoContent();
    }
}