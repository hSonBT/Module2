using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BaseProject.Core.Abstractions;

/// <summary>
/// Base API controller for REST endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseApiController : BaseController
{
    protected BaseApiController(IMediator mediator, ILogger logger) : base(mediator, logger)
    {
    }
}