using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PowerTaskManager.Models;

namespace PowerTaskManager.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(c => c.Description)
            .HasMaxLength(200);
            
        builder.Property(c => c.Color)
            .IsRequired()
            .HasMaxLength(7);
    }
}