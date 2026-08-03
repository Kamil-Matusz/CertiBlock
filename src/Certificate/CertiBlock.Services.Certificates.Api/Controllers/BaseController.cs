using Microsoft.AspNetCore.Mvc;

namespace CertiBlock.Services.Certificates.Api.Controllers;

[ApiController]
[Route(BasePath + "/[controller]")]
public class BaseController : ControllerBase
{
    protected const string BasePath = "certificate-service";

    protected Guid? CurrentUserId
        => Guid.TryParse(User.Identity?.Name, out var userId) ? userId : null;
}