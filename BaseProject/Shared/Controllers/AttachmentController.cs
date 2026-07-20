using BaseProject.Core.Abstractions;
using BaseProject.Shared.Command;
using BaseProject.Shared.Dtos;
using BaseProject.Shared.services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BaseProject.Shared.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttachmentController : BaseApiController
{
    private readonly IAttachmentService _attachmentService;

    public AttachmentController(IMediator mediator, ILogger logger, IAttachmentService attachmentService) : base(
        mediator, logger)
    {
        _attachmentService = attachmentService;
    }

    /// <summary>
    /// Upload attachments
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Upload(
        [FromForm] UploadAttachmentCommand command,
        CancellationToken cancellationToken = default
    )
    {
        command.EnterpriseId = 1; // TODO: from claims
        command.UserId = 1; // TODO: form claims

        return await ExecuteCommand<UploadAttachmentCommand, AttachmentDto>(command);
    }

    /// <summary>
    /// Get attachments by entity (Device, Trip, etc)
    /// GET /api/attachments?type=Device&id=1
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetByAttachable(
        [FromQuery] string type,
        [FromQuery] int id,
        CancellationToken cancellationToken = default
    )
    {
        var attachments = await _attachmentService.GetByAttachableAsync(type, id, cancellationToken);
        return Ok(new { success = true, data = attachments });
    }

    /// <summary>
    /// Download attachment
    /// GET /api/attachments/1/download
    /// </summary>
    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(
        int id, CancellationToken cancellationToken = default)
    {
        var attachment = await _attachmentService.GetByIdAsync(id, cancellationToken);
        if (attachment == null)
            return NotFound();

        var stream = await _attachmentService.DownloadAsync(id, cancellationToken);
        return File(stream, attachment.MimeType, attachment.FileName);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var success = await _attachmentService.DeleteAsync(id, cancellationToken);
        if (!success)
            return NotFound();

        return Ok(new { success = true });
    }
}