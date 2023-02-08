using KF_2_2023.Managers;
using KF_2_2023.Models;
using OneTimeBuckAPI.Core;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var _cors = builder.Configuration.GetSection("KF22023CORS").Get<string[]>();

if (_cors != null && _cors.Length > 0)
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("KF22023CORS", builder => builder.WithOrigins(_cors).AllowAnyHeader().AllowAnyMethod());
    });
}

builder.Services.AddOptions();
var _settings = builder.Configuration.GetSection("settings").Get<SettingsModel>();
builder.Services.Configure<SettingsModel>(builder.Configuration.GetSection("settings"));

var multiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions
{
    EndPoints = { _settings is null ? string.Empty : _settings.RedisEndpoint },
    AbortOnConnectFail = false
});
builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);
builder.Services.AddSingleton(new SampleStreamManager(multiplexer, _settings));
builder.Services.AddHttpContextAccessor();

if (_settings != null && _settings.MinThreadPools > 0)
{
    if (ThreadPool.SetMinThreads(_settings.MinThreadPools, _settings.MinThreadPools))
    {
        var msg = string.Empty;
    }
}

var app = builder.Build();

// Configure the HTTP request pipeline.
// TURNED ON ALWAYS FOR TESTING PURPOSES, WOULD TURN OFF IN PRODUCTION
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<CustomResponseWrapper>();
app.UseCors("KF22023CORS");

app.MapControllers();

app.Run();
