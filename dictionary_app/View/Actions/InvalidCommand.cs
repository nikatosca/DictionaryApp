using System.Threading.Tasks;
using dictionary_app.Interfaces;
using dictionary_app.Present;

namespace dictionary_app.Services.Actions
{
    public class InvalidCommand : ICommand
    {
        private readonly IMenu _menu;

        public InvalidCommand(IMenu menu)
        {
            _menu = menu;
        }

        // Асинхронное выполнение команды с выводом сообщения об ошибке
        public async Task Execute()
        {
            await Task.Run(() => _menu.DisplayMessage("Неверный выбор. Пожалуйста, выберите вариант от 1 до 4."));
        }
    }
}