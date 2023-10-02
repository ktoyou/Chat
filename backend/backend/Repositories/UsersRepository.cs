using backend.Db;
using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories;

public class UsersRepository : AbstractRepository<User>
{
    public UsersRepository(DbApplicationContext context) : base(context) { }
    
    public override async Task Add(User item)
    {
        await _dbApplicationContext.Users.AddAsync(item);
        await _dbApplicationContext.SaveChangesAsync();
    }

    public override async Task Remove(User item)
    {
        _dbApplicationContext.Users.Remove(item);
        await _dbApplicationContext.SaveChangesAsync();
    }

    public override async Task<User> GetByGuid(Guid guid)
        => await _dbApplicationContext.Users.FirstOrDefaultAsync(u => u.Id == guid);

    public override async Task<List<User>> GetAll()
        => await _dbApplicationContext.Users.ToListAsync();

    public async Task<User> GetByName(string name)
        => await _dbApplicationContext.Users.FirstOrDefaultAsync(u => u.Name == name);

    public async Task<User> GetByConnectionId(string connectionId)
        => await _dbApplicationContext.Users.FirstOrDefaultAsync(u => u.ConnectionId == connectionId);

    public async Task<User> GetSystemUser()
        => await _dbApplicationContext.Users.FirstOrDefaultAsync(u => u.Name == "System");

    public async Task UpdateUserConnectionIdByGuid(Guid guid, string connectionId)
    {
        var user = await GetByGuid(guid);
        user.ConnectionId = connectionId;
        await _dbApplicationContext.SaveChangesAsync();
    }
}