using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TaskPro.Domain.Entities;

namespace TaskPro.Infraestructure.Persistence.SQLDB
{
    public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Log> Logs => Set<Log>();
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
    }
}
