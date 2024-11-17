using System;
using System.Threading.Tasks;
using dictionary_app.Interfaces;
using dictionary_app.Present;

namespace dictionary_app.Services.Actions
{
    public class RemoveWordCommand : ICommand
    {
        private readonly IMenu _menu;
        private readonly IDatabase _database;

        public RemoveWordCommand(IMenu menu, IDatabase database)
        {
            _menu = menu;
            _database = database;
        }

        public async Task Execute()
        {
            _menu.DisplayMessage("Введите слово для удаления:");
            var term = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(term))
            {
                _menu.DisplayMessage("Вы не ввели слово для удаления.");
                return;
            }

            try
            {
                if (await _database.ContainsAsync(term))
                {
                    await _database.RemoveWordAsync(term);
                    _menu.DisplayMessage($"Слово '{term}' было успешно удалено.");
                }
                else
                {
                    _menu.DisplayMessage("Такого слова нет в словаре.");
                }
            }
            catch (Exception ex)
            {
                // Обрабатываем ошибки, если они возникнут
                _menu.DisplayMessage($"Ошибка при удалении слова: {ex.Message}");
            }
        }
    }
}