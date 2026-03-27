using TaskPro.Domain.Interfaces.Repositories;

namespace TaskPro.Infraestructure.Persistence.SQLDB.Repositories
{
    public class UnitOfWork(ApplicationDbContext context,
                              IUsersRepository usersRepository,
                              ITasksRepository tasksRepository,
                              IProjectsRepository projectsRepository) : IUnitOfWork
    {
        public IUsersRepository Users => usersRepository;

        public IProjectsRepository Projects => projectsRepository;

        public ITasksRepository Tasks => tasksRepository;

        public async Task<int> CommitAsync(CancellationToken ct = default) => await context.SaveChangesAsync(ct);
        public void Dispose()
        {
            context?.Dispose();
        }
    }
}
