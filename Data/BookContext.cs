using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Book.Data
{
    public class BookContext : DbContext
    {
        public DbSet<Books> Books { get; set; }

        public BookContext(DbContextOptions<BookContext> options) : base(options)
        {
        }
        public BookContext() {}
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
        

        public static BookContext Init()
        {
            try
            {
                // Create configuration
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory()) // Set the current directory as the base path
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false); // Load the configuration from JSON

                var configuration = builder.Build();

                // Create the service collection
                var services = new ServiceCollection();

                // Add DbContext
                services.AddDbContext<BookContext>(options =>
                    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

                // Build the service provider
                var serviceProvider = services.BuildServiceProvider();
                var dbContext = serviceProvider.GetRequiredService<BookContext>();
                
                // Ensure the database is created
                var dbCreated = dbContext.Database.EnsureCreated();
                if (dbCreated)
                {
                    Console.WriteLine("База данных успешно создана.");
                }
                else
                {
                    Console.WriteLine("База данных уже существует.");
                    
                }

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
}