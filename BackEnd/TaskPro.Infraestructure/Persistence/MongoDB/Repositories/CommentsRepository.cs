using MongoDB.Driver;
using TaskPro.Domain.Entities;
using TaskPro.Domain.Interfaces.Repositories;

namespace TaskPro.Infraestructure.Persistence.MongoDB.Repositories
{
    internal class CommentsRepository(MongoDbContext mongoContext) : ICommentsRepository
    {
        private readonly IMongoCollection<Comment> _collection
            = mongoContext.GetCollection<Comment>("Comments");

        public async Task<Comment?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var filter = Builders<Comment>.Filter.Eq(c => c.Id, id);
            return await _collection.Find(filter).FirstOrDefaultAsync(ct);
        }

        public async Task<IEnumerable<Comment>> GetByTaskAsync(Guid taskId, CancellationToken ct = default)
        {
            var filter = Builders<Comment>.Filter.Eq(c => c.TaskId, taskId);

            return await _collection
                .Find(filter)
                .SortByDescending(c => c.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task AddAsync(Comment comment, CancellationToken ct = default)
            => await _collection.InsertOneAsync(comment, cancellationToken: ct);

        public async Task UpdateAsync(Comment comment, CancellationToken ct = default)
        {
            var filter = Builders<Comment>.Filter.Eq(c => c.Id, comment.Id);
            await _collection.ReplaceOneAsync(filter, comment, cancellationToken: ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var filter = Builders<Comment>.Filter.Eq(c => c.Id, id);
            await _collection.DeleteOneAsync(filter, ct);
        }
    }
}
