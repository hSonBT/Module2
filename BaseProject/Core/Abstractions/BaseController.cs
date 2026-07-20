using AutoMapper;
using BaseProject.Core.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BaseProject.Core.Abstractions;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected readonly IMediator Mediator;
    protected readonly ILogger Logger;

    protected BaseController(IMediator mediator, ILogger logger)
    {
        Mediator = mediator;
        Logger = logger;
    }

    /// <summary>
    /// Replaces Laravel`s actionPost() - Execute command and return response
    /// </summary>
    protected async Task<IActionResult> ExecuteCommand<TRequest, TResponse>(
        TRequest request
    ) where TRequest : MediatR.IRequest<TResponse>
    {
        try
        {
            var result = await Mediator.Send(request);
            return Ok(ResponseBase<TResponse>.SuccessResponse(result));
        }
        catch (Exceptions.ValidationException ex)
        {
            return BadRequest(ResponseBase<TResponse>.ErrorResponse(ex.Message, ex.Error));
        }
        catch (Exceptions.NotFoundException ex)
        {
            return NotFound(ResponseBase<TResponse>.ErrorResponse(ex.Message));
        }
        catch (System.Exception ex)
        {
            Logger.LogError(ex, "Error executing command");
            return StatusCode(500, ResponseBase<TResponse>.ErrorResponse("An error occurred"));
        }
    }

    /// <summary>
    /// Replaces Laravel`s action()  - Execute query return response
    /// </summary>
    protected async Task<IActionResult> ExecuteQuery<TRequest, TResponse>(
        TRequest request
    ) where TRequest : Models.IRequest<TResponse>
        where TResponse : class
    {
        try
        {
            var result = await Mediator.Send(request);
            if (result == null)
            {
                return NotFound(ResponseBase<TResponse>.NotFoundResponse());
            }

            return Ok(ResponseBase<TResponse>.SuccessResponse(result));
        }
        catch (Exceptions.ValidationException ex)
        {
            return BadRequest(ResponseBase<TResponse>.ErrorResponse(ex.Message, ex.Error));
        }
        catch (Exceptions.NotFoundException ex)
        {
            return NotFound(ResponseBase<TResponse>.ErrorResponse(ex.Message));
        }
        catch (System.Exception ex)
        {
            Logger.LogError(ex, "Error executing command");
            return StatusCode(500, ResponseBase<TResponse>.ErrorResponse("An error occurred"));
        }
    }


    /// <summary>
    /// Helper for JSON response (replaces Laravel`s json())
    /// </summary>
    protected IActionResult JsonOk<T>(T data, string message = "Success") =>
        Ok(ResponseBase<T>.SuccessResponse(data, message));

    protected IActionResult JsonError<T>(string message, int statusCode = 400) =>
        StatusCode(statusCode, ResponseBase<T>.ErrorResponse(message));
}