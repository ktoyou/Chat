using backend.Db;
using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories;

public class RoomsRepository : AbstractRepository<Room>
{
    public RoomsRepository(DbApplicationContext context) : base(context) { }

    public override async Task AddAsync(Room item)
    {
        await _dbApplicationContext.Rooms.AddAsync(item);
        await _dbApplicationContext.SaveChangesAsync();
    }

    public override async Task RemoveAsync(Room item)
    {
        _dbApplicationContext.Rooms.Remove(item);
        await _dbApplicationContext.SaveChangesAsync();
    }
    
    public override async Task<Room?> GetByGuidAsync(Guid guid)
    {
        var room = await _dbApplicationContext.Rooms
            .Include(u => u.Users)
            .Include(m => m.Messages)
            .FirstOrDefaultAsync(r => r.Id == guid);
        return room;
    }

    public override async Task<List<Room>> GetAllAsync()
    {
        var rooms = await _dbApplicationContext.Rooms
            .Include(u => u.Users)
            .Include(m => m.Messages)
            .ToListAsync();
        return  rooms;
    }

    public async Task<Room?> GetByName(string name)
        => await _dbApplicationContext.Rooms
            .FirstOrDefaultAsync(r => r.Name == name);
    
    public async Task<List<Guid>> GetRoomsIdsByUser(User user) 
        => await _dbApplicationContext.Rooms
            .Include(r => r.Users)
            .Where(r => r.Users.FirstOrDefault(u => u.Id == user.Id) != null)
            .Select(r => r.Id)
            .ToListAsync();

    public async Task RemoveUserFromAllRooms(User user)
    {
        await _dbApplicationContext.Rooms.Include(r => r.Users).ForEachAsync(room => room.Users.Remove(user));
        await _dbApplicationContext.SaveChangesAsync();
    }
}