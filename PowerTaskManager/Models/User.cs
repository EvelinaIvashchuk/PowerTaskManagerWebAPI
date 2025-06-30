using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace PowerTaskManager.Models;

public class User : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    // Navigation properties
    public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
