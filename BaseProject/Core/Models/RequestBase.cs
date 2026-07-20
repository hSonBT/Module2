namespace BaseProject.Core.Models;

/// <summary>
/// Base class for all Commands and Queries
/// </summary>
/// <typeparam name="TResponse"></typeparam>
public abstract class RequestBase<TResponse> : IRequest<TResponse>
{
    public int? UserId { get; set; }
    public int? EnterpriseId { get; set; }
}