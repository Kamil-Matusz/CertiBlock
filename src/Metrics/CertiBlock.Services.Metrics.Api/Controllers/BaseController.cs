using Microsoft.AspNetCore.Mvc;

namespace CertiBlock.Services.Metrics.Api.Controllers;

[ApiController]
[Route(BasePath + "/[controller]")]
public class BaseController : ControllerBase
{
    protected const string BasePath = "metrics-service";
}
