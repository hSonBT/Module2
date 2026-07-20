namespace BaseProject.Core.Models;

/// <summary>
/// Replaces Laravel Action::handle()
/// Represents a command or query that returns a response
/// </summary>
/// <typeparam name="TResponse"></typeparam>
public interface IRequest<out TResponse> : MediatR.IRequest<TResponse>
{
}