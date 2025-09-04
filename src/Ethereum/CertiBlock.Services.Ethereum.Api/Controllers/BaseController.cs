using Microsoft.AspNetCore.Mvc;

namespace CertiBlock.Services.Ethereum.Api.Controllers;

[ApiController]
[Route(BasePath + "/[controller]")]
public class BaseController : ControllerBase
{
    protected const string BasePath = "ethereum-service";
}