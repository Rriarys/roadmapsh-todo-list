using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDoList.Api.Models;

namespace ToDoList.Api.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.Name).IsRequired().HasMaxLength(100);

        builder.Property(u => u.Email).IsRequired().HasMaxLength(255);

        builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(512); ;

        builder.HasIndex(u => u.Email).IsUnique();  // for unique email addresses and to improve query performance
    }
}
