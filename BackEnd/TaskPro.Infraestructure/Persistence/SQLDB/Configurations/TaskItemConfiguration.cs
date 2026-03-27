using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskPro.Domain.Entities;

namespace TaskPro.Infraestructure.Persistence.SQLDB.Configurations
{
    public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
    {
        public void Configure(EntityTypeBuilder<TaskItem> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                   .HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(t => t.Title)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(t => t.Description)
                   .HasMaxLength(2000)
                   .IsRequired(false);

            builder.Property(t => t.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(t => t.DueDate).IsRequired(false);
            builder.Property(t => t.CreatedAt).IsRequired();
            builder.Property(t => t.UpdatedAt).IsRequired(false);

            builder.HasOne(t => t.Project)
                   .WithMany(p => p.Tasks)
                   .HasForeignKey(t => t.ProjectId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.AssignedTo)
                   .WithMany(pm => pm.AssignedTasks)
                   .HasForeignKey(t => t.AssignedToMemberId)
                   .OnDelete(DeleteBehavior.NoAction)
                   .IsRequired(false);


            builder.HasIndex(t => t.ProjectId)
                   .HasDatabaseName("IX_TaskItems_ProjectId");

            builder.HasIndex(t => t.AssignedToMemberId)
                   .HasDatabaseName("IX_TaskItems_AssignedToMemberId");

            builder.ToTable("TaskItems");
        }
    }
}
