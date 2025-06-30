using System.ComponentModel.DataAnnotations;

namespace PowerTaskManager.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Color { get; set; } = "#000000"; // Default color is black
    
    // Navigation properties
    public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}