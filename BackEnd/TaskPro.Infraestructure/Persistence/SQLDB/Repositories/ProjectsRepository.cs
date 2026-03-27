using Microsoft.EntityFrameworkCore;
using TaskPro.Domain.Entities;
using TaskPro.Domain.Interfaces.Repositories;

namespace TaskPro.Infraestructure.Persistence.SQLDB.Repositories
{
    public class ProjectsRepository(ApplicationDbContext context) : IProjectsRepository
    {
        public async Task<IEnumerable<Project>> GetAllAsync(CancellationToken ct = default) =>
                            await context.Projects.Include(p => p.Members)
                            .ThenInclude(m => m.User)
                            .Include(p => p.Tasks)
                            .ToListAsync(ct);
        public async Task<Project?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await context.Projects
                            .FirstOrDefaultAsync(p => p.Id == id, ct);

        public async Task<Project?> GetByIdWithMembersAsync(Guid id, CancellationToken ct = default)
            => await context.Projects
                            .Include(p => p.Members)
                                .ThenInclude(m => m.User)
                            .Include(p => p.Tasks)
                            .FirstOrDefaultAsync(p => p.Id == id, ct);

        public async Task<IEnumerable<Project>> GetByUserAsync(Guid userId, CancellationToken ct = default)
            => await context.Projects
                            .Include(p => p.Members)
                            .ThenInclude(m => m.User)
                            .Include(p => p.Tasks)
                            .Where(p => p.Members.Any(m => m.UserId == userId))
                            .ToListAsync(ct);

        public async Task<ProjectMember?> GetMemberAsync(Guid projectId, Guid userId, CancellationToken ct = default)
            => await context.ProjectMembers
                            .Include(pm => pm.User)
                            .FirstOrDefaultAsync(pm => pm.ProjectId == projectId
                                                    && pm.UserId == userId, ct);

        public async Task AddAsync(Project project, CancellationToken ct = default)
            => await context.Projects.AddAsync(project, ct);

        public void Update(Project project)
            => context.Projects.Update(project);

        public void Delete(Project project)
            => context.Projects.Remove(project);
    }
}

