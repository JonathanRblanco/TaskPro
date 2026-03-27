using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskPro.Domain.Interfaces.Repositories;
using TaskPro.Infraestructure.Persistence.MongoDB;
using TaskPro.Infraestructure.Persistence.MongoDB.Repositories;
using TaskPro.Infraestructure.Persistence.SQLDB;
using TaskPro.Infraestructure.Persistence.SQLDB.Repositories;
using TaskPro.Infraestructure.Settings;

namespace TaskPro.Infraestructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("SqlServer"),
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);
                }));

            services.Configure<MongoDbSettings>(options => configuration.GetSection("MongoDB").Bind(options));
            services.AddSingleton<MongoDbContext>();

            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IProjectsRepository, ProjectsRepository>();
            services.AddScoped<ITasksRepository, TasksRepository>();
            services.AddScoped<ICommentsRepository, CommentsRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
