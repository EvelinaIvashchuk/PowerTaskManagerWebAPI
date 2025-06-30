using FluentValidation;

namespace PowerTaskManager.DTOs.Validators;

public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(50).WithMessage("Name cannot exceed 50 characters");
            
        RuleFor(x => x.Description)
            .MaximumLength(200).WithMessage("Description cannot exceed 200 characters");
            
        RuleFor(x => x.Color)
            .NotEmpty().WithMessage("Color is required")
            .MaximumLength(7).WithMessage("Color cannot exceed 7 characters")
            .Matches("^#[0-9A-Fa-f]{6}$").WithMessage("Color must be a valid hex color code (e.g., #FF0000)");
    }
}

public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
{
    public UpdateCategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(50).WithMessage("Name cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.Name));
            
        RuleFor(x => x.Description)
            .MaximumLength(200).WithMessage("Description cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));
            
        RuleFor(x => x.Color)
            .MaximumLength(7).WithMessage("Color cannot exceed 7 characters")
            .Matches("^#[0-9A-Fa-f]{6}$").WithMessage("Color must be a valid hex color code (e.g., #FF0000)")
            .When(x => !string.IsNullOrEmpty(x.Color));
    }
}

public class CategoryQueryParametersValidator : AbstractValidator<CategoryQueryParameters>
{
    public CategoryQueryParametersValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("Page number must be greater than or equal to 1");
            
        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("Page size must be greater than or equal to 1")
            .LessThanOrEqualTo(50).WithMessage("Page size must be less than or equal to 50");
    }
}