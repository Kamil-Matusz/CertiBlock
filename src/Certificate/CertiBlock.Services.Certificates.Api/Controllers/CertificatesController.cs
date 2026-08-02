using CertiBlock.Services.Certificates.Core.DTO;
using CertiBlock.Services.Certificates.Core.Services;
using CertiBlock.Shared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CertiBlock.Services.Certificates.Api.Controllers;

//[Authorize]
public class CertificatesController(ICertificateService certificateService) : BaseController
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Register([FromBody] CertificateRequest request)
    {
        var user = UserContextProvider.FromClaimsPrincipal(User);
        var response = await certificateService.RegisterCertificateAsync(request, user);
        return CreatedAtAction(nameof(GetCertificateById), new { id = response.CertificateId }, response);
    }
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCertificateById(Guid id)
        => Ok(await certificateService.GetCertificateByIdAsync(id));

    [HttpGet("getCertificatesByUserId/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCertificatesByUser()
    {
        var userId = CurrentUserId;
        if (userId is null)
        {
            return NotFound();
        }

        var certificates = await certificateService.GetCertificatesByUserIdAsync(userId.Value);
        return Ok(certificates);
    }
    
    [HttpGet("getAllCertificates")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllCertificates()
        => Ok(await certificateService.GetAllCertificatesAsync());
    
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCertificateById(Guid id)
    {
        await certificateService.DeleteCertificateAsync(id);
        return NoContent();
    }
    
    [HttpGet("getCertificates")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CertificateDto>> GetEthereumTransactionsPaged([FromQuery] int pageIndex, [FromQuery] int pageSize)
    {
        var (page, size) = Paging.Normalize(pageIndex, pageSize);
        return Ok(await certificateService.GetCertificatesPagedAsync(page, size));
    }
    
    [HttpGet("countCertificates")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CountCertificates()
        => Ok(await certificateService.GetCertificateCountAsync());
}