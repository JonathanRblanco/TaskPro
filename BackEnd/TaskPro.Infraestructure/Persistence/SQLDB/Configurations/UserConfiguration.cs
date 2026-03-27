using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskPro.Domain.Entities;

namespace TaskPro.Infraestructure.Persistence.SQLDB.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                   .HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.OwnsOne(u => u.Name, nameBuilder =>
            {
                nameBuilder.Property(n => n.FirstName)
                           .HasColumnName("FirstName")
                           .HasMaxLength(100)
                           .IsRequired();

                nameBuilder.Property(n => n.LastName)
                           .HasColumnName("LastName")
                           .HasMaxLength(100)
                           .IsRequired();
                nameBuilder.Ignore(n => n.DisplayName);
            });

            builder.OwnsOne(u => u.Email, emailBuilder =>
            {
                emailBuilder.Property(e => e.Value)
                            .HasColumnName("Email")
                            .HasMaxLength(255)
                            .IsRequired();

                emailBuilder.HasIndex(e => e.Value)
                            .IsUnique()
                            .HasDatabaseName("IX_Users_Email");
            });

            builder.Property(u => u.PasswordHash)
                   .IsRequired();

            builder.Property(u => u.Role)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(u => u.CreatedAt)
                   .IsRequired();

            builder.Property(u => u.UpdatedAt)
                   .IsRequired(false);

            builder.ToTable("Users");
        }
    }
}
