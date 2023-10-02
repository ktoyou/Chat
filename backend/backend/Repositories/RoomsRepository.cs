using backend.Db;
using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories;

public class RoomsRepository : AbstractRepository<Room>
{
    public RoomsRepository(DbApplicationContext context) : base(context) { }

    public override async Task Add(Room item)
    {
        await _dbApplicationContext.Rooms.AddAsync(item);
        await _dbApplicationContext.SaveChangesAsync();
    }

    public override async Task Remove(Room item)
    {
        _dbApplicationContext.Rooms.Remove(item);
        await _dbApplicationContext.SaveChangesAsync();
    }
    //2fd9bf98-73d5-441f-8862-c00bbcb472df
    public override async Task<Room> GetByGuid(Guid guid)
    {
        var room = await _dbApplicationContext.Rooms.Include(u => u.Users).Include(m => m.Messages).FirstOrDefaultAsync(r => r.Id == guid);
        return room;
    }

    public override async Task<List<Room>> GetAll()
    {
        var rooms = await _dbApplicationContext.Rooms.Include(u => u.Users).Include(m => m.Messages).ToListAsync();
        return  rooms;
    }

    public async Task<Room> GetByName(string name)
        => await _dbApplicationContext.Rooms.FirstOrDefaultAsync(r => r.Name == name);
    
    public async Task<List<Guid>> GetRoomsIdsByUser(User user) 
        => await _dbApplicationContext.Rooms.Where(r => r.Users.FirstOrDefault(user) != null).Select(r => r.Id).ToListAsync();

    public async Task RemoveUserFromAllRooms(User user)
    {
        await _dbApplicationContext.Rooms.ForEachAsync(room => room.Users.Remove(user));
        await _dbApplicationContext.SaveChangesAsync();
    }
}