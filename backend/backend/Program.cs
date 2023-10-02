using backend.Controllers;
using backend.Db;
using backend.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DbApplicationContext>();
builder.Services.AddTransient<UsersRepository>();
builder.Services.AddTransient<RoomsRepository>();
builder.Services.AddTransient<MessagesRepository>();

builder.Services.AddSignalR(builder =>
{
    builder.MaximumReceiveMessageSize = null;
    builder.EnableDetailedErrors = true;
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        b =>
        {
            b.WithOrigins(builder.Configuration.GetValue<string>("Origin") ?? string.Empty)
                .AllowAnyHeader()
                .WithMethods("GET", "POST")
                .AllowCredentials();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseAuthentication();

app.UseCors();

app.MapHub<ChatHub>("/chat");

app.MapControllers();

app.Run();