using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskPro.Domain.Entities;

namespace TaskPro.Infraestructure.Persistence.SQLDB.Configurations
{
    public class LogConfiguration : IEntityTypeConfiguration<Log>
    {
        public void Configure(EntityTypeBuilder<Log> builder)
        {
            builder.HasKey(l => l.Id);

            builder.Property(l => l.Message).HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(l => l.Level).HasMaxLength(10).IsRequired();
            builder.Property(l => l.StackTrace).HasColumnType("nvarchar(max)");
            builder.Property(l => l.Logger).HasMaxLength(500);
            builder.Property(l => l.Url).HasMaxLength(2000);
            builder.Property(l => l.Exception).HasColumnType("nvarchar(max)");
            builder.Property(l => l.InnerException).HasColumnType("nvarchar(max)");
            builder.Property(l => l.CreatedOn).IsRequired();
            builder.Property(l => l.Controller).HasMaxLength(200);
            builder.Property(l => l.Action).HasMaxLength(200);
            builder.Property(l => l.Method).HasMaxLength(10);
            builder.Property(l => l.User).HasMaxLength(200);
            builder.Property(l => l.Ip).HasMaxLength(50);
            builder.Property(l => l.SourceURL).HasMaxLength(2000);

            builder.ToTable("Logs");
        }
    }
}
