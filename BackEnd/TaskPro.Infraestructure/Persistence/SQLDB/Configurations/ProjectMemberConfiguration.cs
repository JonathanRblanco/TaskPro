using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskPro.Domain.Entities;

namespace TaskPro.Infraestructure.Persistence.SQLDB.Configurations
{
    public class ProjectMemberConfiguration : IEntityTypeConfiguration<ProjectMember>
    {
        public void Configure(EntityTypeBuilder<ProjectMember> builder)
        {
            builder.HasKey(pm => pm.Id);

            builder.Property(pm => pm.Id)
                   .ValueGeneratedNever();

            builder.HasIndex(pm => new { pm.ProjectId, pm.UserId })
                   .IsUnique()
                   .HasDatabaseName("IX_ProjectMembers_ProjectId_UserId");

            builder.Property(pm => pm.Role)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(pm => pm.CreatedAt).IsRequired();
            builder.Property(pm => pm.UpdatedAt).IsRequired(false);

            builder.HasOne(pm => pm.Project)
                   .WithMany(p => p.Members)
                   .HasForeignKey(pm => pm.ProjectId)
                   .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(pm => pm.User)
                   .WithMany(u => u.Memberships)
                   .HasForeignKey(pm => pm.UserId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.ToTable("ProjectMembers");
        }
    }
}
