using backend.Db;
using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories;

public class MessagesRepository : AbstractRepository<Message>
{
    public MessagesRepository(DbApplicationContext context) : base(context) {}

    public override async Task AddAsync(Message item)
    {
        await _dbApplicationContext.Messages.AddAsync(item);
        await _dbApplicationContext.SaveChangesAsync();
    }

    public override async Task RemoveAsync(Message item)
    {
        _dbApplicationContext.Messages.Remove(item);
        await _dbApplicationContext.SaveChangesAsync();
    }

    public override async Task<Message?> GetByGuidAsync(Guid guid)
        => await _dbApplicationContext.Messages.FirstOrDefaultAsync(m => m.Id == guid);

    public override async Task<List<Message>> GetAllAsync()
        => await _dbApplicationContext.Messages.ToListAsync();

    public async Task<List<Message>> GetMessagesByRoomGuid(Guid guid)
        => await _dbApplicationContext.Messages
            .Include(m => m.Room)
            .Include(u => u.User)
            .Where(m => m.Room.Id == guid)
            .ToListAsync();
}