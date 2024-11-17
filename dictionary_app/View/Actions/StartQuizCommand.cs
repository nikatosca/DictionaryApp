using dictionary_app.Interfaces;
using dictionary_app.Present;

namespace dictionary_app.Services.Actions
{
    public class StartQuizCommand : ICommand
    {
        private readonly IMenu _menu;
        private readonly IDatabase _database;

        public StartQuizCommand(IMenu menu, IDatabase database)
        {
            _menu = menu;
            _database = database;
        }

        public async Task Execute()
        {
            var words = await _database.GetAllWordsAsync();

            if (words.Count == 0)
            {
                _menu.DisplayMessage("Словарь пуст. Добавьте слова для начала тестирования.");
                return;
            }

            _menu.DisplayMessage("Начинаем тестирование! Введите 'exit' для выхода в главное меню.");
            while (true)
            {
                var word = await _database.GetRandomWordAsync();
                if (word == null)
                {
                    _menu.DisplayMessage("Словарь пуст. Добавьте слова для продолжения тестирования.");
                    break;
                }

                string userAnswer = _menu.Prompt($"Переведите слово: {word.Original}\nВаш ответ: ").Trim();

                if (userAnswer.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                bool isCorrect = userAnswer.Equals(word.Meaning, StringComparison.OrdinalIgnoreCase);
                _menu.DisplayMessage(isCorrect ? "Правильно!" : $"Неправильно. Правильный ответ: {word.Meaning}");

                // Изменение уровня запоминания
                word.Level = isCorrect && word.Level < 3 ? word.Level + 1 : Math.Max(1, word.Level - 1);
                await _database.SaveWordsAsync(new List<Word> { word });
            }
        }
    }
}