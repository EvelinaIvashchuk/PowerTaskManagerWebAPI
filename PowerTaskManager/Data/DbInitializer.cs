using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PowerTaskManager.Models;
using TaskStatus = PowerTaskManager.Models.TaskStatus;

namespace PowerTaskManager.Data;

public static class DbInitializer
{
    public static async Task Initialize(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Ensure database is created and apply migrations
        context.Database.EnsureCreated();
        
        // Seed roles if they don't exist
        if (!await roleManager.Roles.AnyAsync())
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
            await roleManager.CreateAsync(new IdentityRole("User"));
        }
        
        // Seed admin user if it doesn't exist
        if (!await userManager.Users.AnyAsync())
        {
            var adminUser = new User
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true
            };
            
            await userManager.CreateAsync(adminUser, "Admin123!");
            await userManager.AddToRoleAsync(adminUser, "Admin");
            
            var regularUser = new User
            {
                UserName = "user@example.com",
                Email = "user@example.com",
                FirstName = "Regular",
                LastName = "User",
                EmailConfirmed = true
            };
            
            await userManager.CreateAsync(regularUser, "User123!");
            await userManager.AddToRoleAsync(regularUser, "User");
        }
        
        // Seed categories if they don't exist
        if (!await context.Categories.AnyAsync())
        {
            var categories = new List<Category>
            {
                new Category { Name = "Work", Description = "Work-related tasks", Color = "#FF0000" },
                new Category { Name = "Personal", Description = "Personal tasks", Color = "#00FF00" },
                new Category { Name = "Study", Description = "Study-related tasks", Color = "#0000FF" },
                new Category { Name = "Health", Description = "Health and fitness tasks", Color = "#FFFF00" }
            };
            
            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }
        
        // Seed tasks if they don't exist
        if (!await context.Tasks.AnyAsync())
        {
            var admin = await userManager.FindByEmailAsync("admin@example.com");
            var user = await userManager.FindByEmailAsync("user@example.com");
            var categories = await context.Categories.ToListAsync();
            
            var tasks = new List<TaskItem>
            {
                new TaskItem
                {
                    Title = "Complete project documentation",
                    Description = "Write comprehensive documentation for the project",
                    DueDate = DateTime.Now.AddDays(7),
                    Priority = TaskPriority.High,
                    Status = TaskStatus.NotStarted,
                    UserId = admin.Id,
                    CategoryId = categories.First(c => c.Name == "Work").Id
                },
                new TaskItem
                {
                    Title = "Prepare presentation",
                    Description = "Create slides for the upcoming meeting",
                    DueDate = DateTime.Now.AddDays(3),
                    Priority = TaskPriority.Medium,
                    Status = TaskStatus.InProgress,
                    UserId = admin.Id,
                    CategoryId = categories.First(c => c.Name == "Work").Id
                },
                new TaskItem
                {
                    Title = "Go grocery shopping",
                    Description = "Buy groceries for the week",
                    DueDate = DateTime.Now.AddDays(1),
                    Priority = TaskPriority.Low,
                    Status = TaskStatus.NotStarted,
                    UserId = user.Id,
                    CategoryId = categories.First(c => c.Name == "Personal").Id
                },
                new TaskItem
                {
                    Title = "Study for exam",
                    Description = "Review materials for the upcoming exam",
                    DueDate = DateTime.Now.AddDays(5),
                    Priority = TaskPriority.High,
                    Status = TaskStatus.NotStarted,
                    UserId = user.Id,
                    CategoryId = categories.First(c => c.Name == "Study").Id
                }
            };
            
            await context.Tasks.AddRangeAsync(tasks);
            await context.SaveChangesAsync();
        }
    }
}