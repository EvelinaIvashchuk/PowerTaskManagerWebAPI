using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PowerTaskManager.Models;

namespace PowerTaskManager.Data.Configurations;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(t => t.Description)
            .HasMaxLength(500);
            
        builder.Property(t => t.DueDate)
            .IsRequired();
            
        builder.Property(t => t.Priority)
            .IsRequired();
            
        builder.Property(t => t.Status)
            .IsRequired();
            
        builder.Property(t => t.CreatedAt)
            .IsRequired();
            
        // Relationships
        builder.HasOne(t => t.User)
            .WithMany(u => u.Tasks)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.SetNull);
            
        builder.HasOne(t => t.Category)
            .WithMany(c => c.Tasks)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}