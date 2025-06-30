using FluentValidation;
using PowerTaskManager.Models;
using TaskStatus = PowerTaskManager.Models.TaskStatus;

namespace PowerTaskManager.DTOs.Validators;

public class CreateTaskItemDtoValidator : AbstractValidator<CreateTaskItemDto>
{
    public CreateTaskItemDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(100).WithMessage("Title cannot exceed 100 characters");
            
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
            
        RuleFor(x => x.DueDate)
            .NotEmpty().WithMessage("Due date is required")
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Due date must be in the future");
            
        RuleFor(x => x.Priority)
            .NotEmpty().WithMessage("Priority is required")
            .Must(BeValidPriority).WithMessage("Priority must be one of: Low, Medium, High, Urgent");
    }
    
    private bool BeValidPriority(string priority)
    {
        return Enum.TryParse<TaskPriority>(priority, true, out _);
    }
}

public class UpdateTaskItemDtoValidator : AbstractValidator<UpdateTaskItemDto>
{
    public UpdateTaskItemDtoValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(100).WithMessage("Title cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Title));
            
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));
            
        RuleFor(x => x.DueDate)
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Due date must be in the future")
            .When(x => x.DueDate.HasValue);
            
        RuleFor(x => x.Priority)
            .Must(BeValidPriority).WithMessage("Priority must be one of: Low, Medium, High, Urgent")
            .When(x => !string.IsNullOrEmpty(x.Priority));
            
        RuleFor(x => x.Status)
            .Must(BeValidStatus).WithMessage("Status must be one of: NotStarted, InProgress, Completed, Cancelled")
            .When(x => !string.IsNullOrEmpty(x.Status));
    }
    
    private bool BeValidPriority(string priority)
    {
        return Enum.TryParse<TaskPriority>(priority, true, out _);
    }
    
    private bool BeValidStatus(string status)
    {
        return Enum.TryParse<TaskStatus>(status, true, out _);
    }
}

public class TaskItemQueryParametersValidator : AbstractValidator<TaskItemQueryParameters>
{
    public TaskItemQueryParametersValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("Page number must be greater than or equal to 1");
            
        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("Page size must be greater than or equal to 1")
            .LessThanOrEqualTo(50).WithMessage("Page size must be less than or equal to 50");
            
        RuleFor(x => x.Status)
            .Must(BeValidStatus).WithMessage("Status must be one of: NotStarted, InProgress, Completed, Cancelled")
            .When(x => !string.IsNullOrEmpty(x.Status));
            
        RuleFor(x => x.Priority)
            .Must(BeValidPriority).WithMessage("Priority must be one of: Low, Medium, High, Urgent")
            .When(x => !string.IsNullOrEmpty(x.Priority));
    }
    
    private bool BeValidPriority(string priority)
    {
        return string.IsNullOrEmpty(priority) || Enum.TryParse<TaskPriority>(priority, true, out _);
    }
    
    private bool BeValidStatus(string status)
    {
        return string.IsNullOrEmpty(status) || Enum.TryParse<TaskStatus>(status, true, out _);
    }
}