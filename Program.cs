using Challenge_DEV_2023.Models;
using Challenge_DEV_2023.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Register ApiSettings as a singleton and ApiService as Scoped
builder.Services.AddSingleton<DevChallengeApiSettings>();
builder.Services.AddSingleton(service => new DevChallengeApiService(new HttpClient()));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Dev Challenge 2023", Version = "v1" });
});

// Build app
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dev Challenge 2023");
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();