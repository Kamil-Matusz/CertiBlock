using CertiBlock.Services.Users.Core.Entities;
using CertiBlock.Services.Users.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CertiBlock.Services.Users.Infrastructure.DAL.PostgreSQL.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.UserId);
        
        builder.HasIndex(x => x.Email).IsUnique();
        builder.Property(x => x.Email).IsRequired().HasMaxLength(100);
        
        builder.Property(x => x.Password).IsRequired().HasMaxLength(200);
        
        builder.Property(x => x.Role)
            .HasConversion(x => x.Value, x => new Role(x))
            .IsRequired()
            .HasMaxLength(30);
        
        builder.Property(x => x.IsActive).IsRequired();
        
        builder.Property(x => x.CreatedAt).IsRequired();
    }
}