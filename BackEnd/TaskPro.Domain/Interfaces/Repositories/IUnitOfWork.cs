namespace TaskPro.Domain.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUsersRepository Users { get; }
        IProjectsRepository Projects { get; }
        ITasksRepository Tasks { get; }

        Task<int> CommitAsync(CancellationToken ct = default);
    }
}
