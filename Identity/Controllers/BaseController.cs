using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[Produces("application/json")]
public class BaseController : ControllerBase
{
}