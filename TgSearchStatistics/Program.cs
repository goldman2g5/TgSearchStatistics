using Microsoft.EntityFrameworkCore;
using TgSearchStatistics.Models.BaseModels;
using TgSearchStatistics.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TgDbContext>(o => o.UseLazyLoadingProxies().UseNpgsql(builder.Configuration.GetConnectionString("MainConnectionString")));

builder.Services.AddSingleton<IDbContextFactory<TgDbContext>>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("MainConnectionString");

    var optionsBuilder = new DbContextOptionsBuilder<TgDbContext>();
    optionsBuilder.UseNpgsql(connectionString).UseLazyLoadingProxies();

    return new MyDbContextFactory(optionsBuilder.Options, serviceProvider.GetRequiredService<IServiceScopeFactory>());
});

builder.Services.AddSingleton<TgClientFactory>();
builder.Services.AddSingleton<TelegramClientService>();
builder.Services.AddHostedService<TelegramClientInitializer>();

var app = builder.Build();

// Configure the HTTP request pipeline.



app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


public partial class Program { }