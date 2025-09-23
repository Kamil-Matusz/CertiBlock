using Microsoft.AspNetCore.Mvc;

namespace CertiBlock.Services.Polygon.Api.Controllers;

[ApiController]
[Route(BasePath + "/[controller]")]
public class BaseController : ControllerBase
{
    protected const string BasePath = "polygon-service";
}