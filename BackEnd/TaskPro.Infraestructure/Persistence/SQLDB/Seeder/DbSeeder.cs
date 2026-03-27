using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskPro.Domain.Entities;
using TaskPro.Domain.Enums;

namespace TaskPro.Infraestructure.Persistence.SQLDB.Seeder
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (await context.Users.AnyAsync()) return;

            var admin = User.Create(
                firstName: "Admin",
                lastName: "TaskPro",
                email: "admin@taskpro.com",
                passwordHash: BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                role: ApplicationRole.Admin
            );

            var member = User.Create(
                firstName: "Juan",
                lastName: "Pérez",
                email: "juan@taskpro.com",
                passwordHash: BCrypt.Net.BCrypt.HashPassword("Member123!"),
                role: ApplicationRole.Member
            );

            await context.Users.AddRangeAsync(admin, member);
            await context.SaveChangesAsync();
        }
    }
}
