using Book.Commands;
using Book.Data;
using Book.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Подключение к appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", true, false);

// Добавление DbContext
builder.Services.AddDbContext<BookContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Регистрация репозитория и валидатора
builder.Services.AddScoped<IDataRepository, DatabaseConnection>();
builder.Services.AddScoped<IValidator, BookValidator>();

// Добавление контроллеров
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

// Добавление Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Конфигурация Middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
        options.RoutePrefix = "swagger"; // Swagger доступен по корневому URL
    });
}

app.UseHttpsRedirection();

// Добавление маршрутов для контроллеров
app.MapControllers();

app.Run();