using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Db;

public class DbApplicationContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public DbSet<Room> Rooms { get; set; }

    public DbSet<Message> Messages { get; set; }
    
    private readonly string? _connectionString;
    
    public DbApplicationContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        if (_connectionString == null)
            throw new ArgumentException("DefaultConnection must be not null");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(_connectionString);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(new User()
        {
            Id = Guid.NewGuid(),
            Name = "System",
            ConnectionId = "none"
        });
        base.OnModelCreating(modelBuilder);
    }
}