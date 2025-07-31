using CertiBlock.Services.Certificates.Core.DTO;
using CertiBlock.Services.Certificates.Core.Helpers;
using CertiBlock.Services.Certificates.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CertiBlock.Services.Certificates.Api.Controllers;

public class CertificatesController : BaseController
{
    private readonly ICertificateService _certificateService;

    public CertificatesController(ICertificateService certificateService)
    {
        _certificateService = certificateService;
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Register([FromBody] CertificateRequest request)
    {
        var user = UserContextProvider.FromClaimsPrincipal(User);
        var response = await _certificateService.RegisterCertificateAsync(request, user);
        return Ok(response);
    }
}