using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataService;
using PlatformService.Data;
using PlatformService.Models;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.WebHost.UseKestrel();
builder.Services.AddGrpc();
builder.Services.AddControllers();
builder.Services.AddHealthChecks(); // Add health checks

if(builder.Environment.IsProduction())
{   
    Console.WriteLine("Using MySQL Database");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseMySql(builder.Configuration.GetConnectionString("PlatformsConn"), new MySqlServerVersion(new Version(8, 0, 21)))
    );
} 
else {
    Console.WriteLine("Using InMem Database");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("InMem")
    );
}
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

Console.WriteLine($"CommandService Endpoint {builder.Configuration["CommandService"]}");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.MapGrpcService<GrpcPlatformService>();
app.MapGet("/protos/platforms.proto", async context =>
{
    await context.Response.WriteAsync(
        File.ReadAllText(Path.Combine(app.Environment.ContentRootPath, "Protos/platforms.proto"))
    );
});

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health"); // Map health checks endpoint

PrepDb.PrepPopulation(app, builder.Environment.IsProduction());

app.Run();