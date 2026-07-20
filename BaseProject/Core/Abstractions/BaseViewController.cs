using System.Security.Claims;
using BaseProject.Core.Exceptions;
using BaseProject.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BaseProject.Core.Abstractions;

/// <summary>
/// Base class for all MVC Controllers (server-renderd views)
/// Replaces Laravel's base controller with view rendering
/// Handles: ViewResult, form validation display, CQRS integration
/// </summary>
public abstract class BaseViewController : Controller
{
    protected readonly IMediator _mediator;
    protected readonly ILogger<BaseViewController> _logger;

    protected BaseViewController(IMediator mediator, ILogger<BaseViewController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Current authenticated user ID from JWT/Session claims
    /// </summary>
    /// <returns></returns>
    protected int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId) ? userId : 0;
    }

    /// <summary>
    /// Current enterprise ID from claim or session
    /// Multi-tenancy scoping -each user belongs to one enterprise
    /// </summary>
    protected int GetCurrentEnterpriseId()
    {
        var enterpriseIdClaim = User.FindFirst("EnterpriseId");
        return enterpriseIdClaim != null && int.TryParse(enterpriseIdClaim.Value, out var enterpriseId)
            ? enterpriseId
            : 1; // Default enterpriseID   
    }

    /// <summary>
    /// Execute CQRS command and return ViewResult
    /// </summary>
    protected async Task<IActionResult> ExecuteCommandView<TRequest, TResponse>(
        TRequest request,
        string successViewName,
        string? redirectAction = null,
        string? redirectController = null,
        object? routeValues = null
    )
    {
        try
        {
            var response = await _mediator.Send(request);
            TempData["SuccessMessage"] = "Operation completed successfully.";

            if (!string.IsNullOrEmpty(redirectAction))
            {
                return RedirectToAction(redirectAction, redirectController, routeValues);
            }

            return View(successViewName, response);
        }
        catch (ValidationException ex)
        {
            foreach (var error in ex.Error)
            {
                ModelState.AddModelError(error.Key, string.Join(",", error.Value));
            }

            _logger.LogWarning("Validation failed: {Errors}", string.Join("; ", ex.Error.SelectMany(e => e.Value)));
            return View(successViewName);
        }
        catch (NotFoundException ex)
        {
            _logger.LogError(ex, "Unexpected error executing command");
            TempData["ErrorMessage"] = "An unexpected error occurred.";
            return View(successViewName);
        }
    }

    /// <summary>
    ///  Execute CQRS query and return ViewResult
    /// </summary>
    protected async Task<TResponse> ExecuteQueryView<TRequest, TResponse>(
        TRequest request) where TRequest : IRequest<TResponse>
    {
        try
        {
            var response = await _mediator.Send(request);
            return response;
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning("Query returned not found: {Message}", ex.Message);
            TempData["ErrorMessage"] = ex.Message;
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing query");
            throw;
        }
    }

    /// <summary>
    /// Render view with success message
    /// </summary>
    protected IActionResult ViewWithSuccess(string message, string viewName, object? model = null)
    {
        TempData["SuccessMessage"] = message;
        return View(viewName, model);
    }

    /// <summary>
    /// Render view with validation errors
    /// </summary>
    protected IActionResult ViewWithValidationErrors(string viewName, Dictionary<string, string[]> errors,
        object? model = null)
    {
        foreach (var error in errors)
        {
            ModelState.AddModelError(error.Key, string.Join(", ", error.Value));
        }

        return View(viewName, model);
    }

    /// <summary>
    /// Redirect to action with success message
    /// </summary>
    protected IActionResult RedirectWithSuccess(string message, string actionName, string? controllerName = null,
        object? routerValue = null)
    {
        TempData["SuccessMessage"] = message;
        return RedirectToAction(actionName, controllerName, routerValue);
    }

    /// <summary>
    /// Redirect to action with error message
    /// </summary>
    protected IActionResult RedirectWithError(string message, string actionName, string? controllerName = null,
        object? routerValue = null)
    {
        TempData["ErrorMessage"] = message;
        return RedirectToAction(actionName, controllerName, routerValue);
    }

    /// <summary>
    /// Redirect to previous page
    /// </summary>
    protected IActionResult BackWithMessage(string message, string messageType = "success")
    {
        var returnUrl = Request.Headers["Referer"].ToString();
        if (string.IsNullOrEmpty(returnUrl))
        {
            returnUrl = "/";
        }

        if (messageType == "success")
        {
            TempData["SuccessMessage"] = message;
        }
        else
        {
            TempData["ErrorMessage"] = message;
        }

        return Redirect(returnUrl);
    }


    /// <summary>
    /// Set ViewData for use in view
    /// </summary>
    protected void SetViewData(string key, object? value)
    {
        ViewData[key] = value;
    }

    /// <summary>
    /// Set breadcrumb navigation
    /// </summary>
    /// <param name="items"></param>
    protected void SetBreadcrumbs(params (string Label, string? Url)[] items)
    {
        var breadcrumbs = items.Select(i => new Dictionary<string, string>
        {
            { "label", i.Label },
            { "url", i.Url ?? "#" }
        }).ToList();

        ViewData["Breadcrumbs"] = breadcrumbs;
    }

    /// <summary>
    /// Add alert message
    /// </summary>
    /// <param name="message"></param>
    /// <param name="type"></param>
    protected void Alert(string message, string type = "info")
    {
        TempData[$"{type}Message"] = message;
    }

    /// <summary>
    /// Check enterprise authorization
    /// </summary>
    protected bool IsAuthorizedForEnterprise(int enterpriseId)
    {
        var currentEnterpriseId = GetCurrentEnterpriseId();
        return currentEnterpriseId == enterpriseId || IsAdmin();
    }

    /// <summary>
    /// Check if user is admin
    /// </summary>
    /// <returns></returns>
    protected bool IsAdmin()
    {
        return User.IsInRole("Admin");
    }


    /// <summary>
    /// Check if user is authenticated
    /// </summary>
    /// <returns></returns>
    protected bool IsAuthenticated()
    {
        return User.Identity?.IsAuthenticated ?? false;
    }

    /// <summary>
    /// Get user's preferred locale
    /// </summary>
    protected string GetUserLocale()
    {
        var locale = User.FindFirst("Locale")?.Value;
        return !string.IsNullOrEmpty(locale) ? locale : "en";
    }

    protected void LogAction(string action, string details = "")
    {
        var userId = GetCurrentUserId();
        var enterpriseId = GetCurrentEnterpriseId();

        _logger.LogInformation(
            "User Action: UserId={UserId}, EnterpriseId={EnterpriseId}, Action={Action}, Details={Details}", userId,
            enterpriseId, action, details);
    }
}