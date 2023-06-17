using Challenge_DEV_2023.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Initialize configuration
var configuration = new ConfigurationBuilder()
    .AddJsonFile("secrets.json")
    .Build();

// Access the api configuration section
var apiSettings = configuration.GetSection("DevChallengeApiSettings");

// Retrieve values from the secret manager
var email = configuration["Email"];
var apiToken = configuration["ApiToken"];

// Register ApiSettings as a singleton
builder.Services.AddSingleton<DevChallengeApiSettings>();

// Build app
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();