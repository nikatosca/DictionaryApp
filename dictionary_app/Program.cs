using Microsoft.EntityFrameworkCore;
using dictionary_app.Services;
using dictionary_app.Interfaces;
using dictionary_app.Present;
using dictionary_app.View;
using System.Threading.Tasks;


namespace dictionary_app
{
    public class Program
    {
        // Точка входа в приложение
        static async Task Main(string[] args)
        {
            IMenu menu = new Menu();  // Загрузка меню
            var context = new AppDbContext(); // Создание контекста БД
            await context.InitializeDatabaseAsync(); // Инициализация базы данных

            IDatabase db = new Database(context);  // Создание БД для записи слов
            CommandFactory commandFactory = new CommandFactory(menu, db); // Генерация команд
            commandFactory.Start();
        }
    }
}
