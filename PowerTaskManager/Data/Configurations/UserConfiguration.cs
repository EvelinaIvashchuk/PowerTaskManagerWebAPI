using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PowerTaskManager.Models;

namespace PowerTaskManager.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Identity already sets up most of the configuration
        // We just need to configure our custom properties
        
        builder.Property(u => u.FirstName)
            .HasMaxLength(50);
            
        builder.Property(u => u.LastName)
            .HasMaxLength(50);
    }
}