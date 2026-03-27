using Microsoft.EntityFrameworkCore;
using TaskPro.Domain.Entities;
using TaskPro.Domain.Interfaces.Repositories;

namespace TaskPro.Infraestructure.Persistence.SQLDB.Repositories
{
    public class TasksRepository(ApplicationDbContext context) : ITasksRepository
    {
        public async Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await context.TaskItems
                            .FirstOrDefaultAsync(t => t.Id == id, ct);
        public async Task<TaskItem?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default)
            => await context.TaskItems
                            .Include(t => t.Project)
                            .Include(t => t.AssignedTo)
                                .ThenInclude(pm => pm != null ? pm.User : null)
                            .FirstOrDefaultAsync(t => t.Id == id, ct);

        public async Task<IEnumerable<TaskItem>> GetByProjectAsync(Guid projectId, CancellationToken ct = default)
            => await context.TaskItems
                            .Include(t => t.Project)
                            .Include(t => t.AssignedTo)
                                .ThenInclude(pm => pm != null ? pm.User : null)
                            .Where(t => t.ProjectId == projectId)
                            .ToListAsync(ct);

        public async Task<IEnumerable<TaskItem>> GetByMemberAsync(Guid memberId, CancellationToken ct = default)
            => await context.TaskItems
                            .Include(t => t.Project)
                            .Where(t => t.AssignedToMemberId == memberId)
                            .ToListAsync(ct);

        public async Task AddAsync(TaskItem task, CancellationToken ct = default)
            => await context.TaskItems.AddAsync(task, ct);

        public void Update(TaskItem task)
            => context.TaskItems.Update(task);

        public void Delete(TaskItem task)
            => context.TaskItems.Remove(task);

        public async Task<IEnumerable<TaskItem>> GetByUserAsync(Guid requestingUserId)
                            => await context.TaskItems
                                .Include(p => p.Project)
                                .Include(t => t.AssignedTo)
                                .ThenInclude(pm => pm != null ? pm.User : null)
                                .Where(t => t.AssignedTo != null && t.AssignedTo.UserId == requestingUserId)
                                .ToListAsync();
    }
}
