using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Book.Data;

public class BookContext : DbContext
{
    public BookContext(DbContextOptions<BookContext> options) : base(options)
    {
    }

    public BookContext()
    {
    }

    // Набор данных для хранения информации о книгах
    public DbSet<Books> Books { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Конфигурация Books
        modelBuilder.Entity<Books>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Title).IsRequired().HasMaxLength(255);
            entity.Property(t => t.Author).IsRequired().HasMaxLength(255);
            entity.Property(t => t.Description).HasMaxLength(1000);
            entity.Property(t => t.ISBN).IsRequired().HasMaxLength(255);
            entity.Property(t => t.Year).IsRequired().HasMaxLength(5);
            entity.Property(b => b.Keywords).HasColumnType("text");
        });
    }

    // Метод для инициализации базы данных
    public static BookContext Init()
    {
        try
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Устанавливаем текущую директорию в качестве базовой
                .AddJsonFile("appsettings.json", true, false);

            var configuration = builder.Build(); // Компилируем конфигурацию

            // Создаём коллекцию сервисов для настройки зависимостей
            var services = new ServiceCollection();

            // Добавляем DbContext 
            services.AddDbContext<BookContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Создаём провайдер сервисов
            var serviceProvider = services.BuildServiceProvider();
            var dbContext = serviceProvider.GetRequiredService<BookContext>();

            // Проверяем, существует ли база данных, и создаём её при необходимости
            var dbCreated = dbContext.Database.EnsureCreated();
            if (dbCreated)
                Console.WriteLine("База данных успешно создана.");
            else
                Console.WriteLine("База данных уже существует.");

            return dbContext;
        }
        catch (FileNotFoundException ex)
        {
            throw new InvalidOperationException($"Файл конфигурации не найден: {ex.Message}");
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException($"Ошибка в формате файла конфигурации: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Ошибка при инициализации базы данных: {ex.Message}");
        }
    }
}