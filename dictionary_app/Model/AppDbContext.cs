using dictionary_app.Services;
using Microsoft.EntityFrameworkCore;
public class AppDbContext: DbContext
{
    public DbSet<Word> Words { get; set; }

    
    // Метод для настройки соединения с БД
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=words.db");
        }
    }

    // Метод для указания имени таблицы
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Указание имени таблицы для модели Word
        modelBuilder.Entity<Word>().ToTable("Words");
    }

    // Метод для инициализации базы данных
    public async Task InitializeDatabaseAsync()
    {
        // Обеспечиваем создание базы данных и ее схемы, если они не существуют
        await Database.EnsureCreatedAsync();
    }
}
