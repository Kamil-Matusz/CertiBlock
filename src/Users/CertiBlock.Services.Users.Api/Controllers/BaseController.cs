using Microsoft.AspNetCore.Mvc;

namespace CertiBlock.Services.Users.Api.Controllers;

[ApiController]
[Route(BasePath + "/[controller]")]
public class BaseController : ControllerBase
{
    protected const string BasePath = "users-service";
}