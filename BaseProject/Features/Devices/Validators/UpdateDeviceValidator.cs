using BaseProject.Features.Devices.Dtos;
using FluentValidation;

namespace BaseProject.Features.Devices.Validators;

/// <summary>
/// Create device validator
/// Replaces Laravel Validate\Create rules()
/// </summary>
public class UpdateDeviceValidator : AbstractValidator<UpdateDeviceDto>
{
    // Constructor
    public UpdateDeviceValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(255).WithMessage("Device name cannot exceed 255 characters")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Device description cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).When(x => x.Latitude.HasValue)
            .WithMessage("Latitude must be between -90 and 90.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).When(x => x.Longitude.HasValue)
            .WithMessage("Longitude must be between -180 and 180");
    }
}