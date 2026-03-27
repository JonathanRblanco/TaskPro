using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskPro.Domain.Entities;

namespace TaskPro.Infraestructure.Persistence.SQLDB.Configurations
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                   .HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(p => p.Name)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(p => p.Description)
                   .HasMaxLength(2000)
                   .IsRequired(false);

            builder.Property(p => p.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(p => p.CreatedAt).IsRequired();
            builder.Property(p => p.UpdatedAt).IsRequired(false);

            builder.HasOne(p => p.Owner)
                   .WithMany()
                   .HasForeignKey(p => p.OwnerId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(p => p.Members)
            .WithOne()
            .HasForeignKey(m => m.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(p => p.Members)
                   .HasField("_members")
                   .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.ToTable("Projects");
        }
    }
}
