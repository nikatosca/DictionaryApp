using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using dictionary_app.Interfaces;
using dictionary_app.Present;
using dictionary_app.Services;

namespace dictionary_app.Services.Actions
{
    public class AddNewWordCommand : ICommand
    {
        private readonly IMenu _menu;
        private readonly IDatabase _database;

        public AddNewWordCommand(IMenu menu, IDatabase database)
        {
            _menu = menu;
            _database = database;
        }

        public async Task Execute()
        {
            try
            {
                // Запрос ввода оригинала слова
                _menu.DisplayMessage("Введите слово на английском:");
                var original = Console.ReadLine()?.Trim();

                // Проверка на пустое значение
                if (string.IsNullOrWhiteSpace(original))
                {
                    _menu.DisplayMessage("Слово не может быть пустым.");
                    return;
                }

                // Проверка, что слово только на латинице, без цифр, символов и трех одинаковых букв подряд
                if (!IsValidWord(original))
                {
                    _menu.DisplayMessage("Слово должно быть на латинице, без цифр, символов и без трех одинаковых букв подряд.");
                    return;
                }

                // Проверка на наличие слова в базе
                if (await _database.ContainsAsync(original))
                {
                    _menu.DisplayMessage($"Слово '{original}' уже есть в словаре.");
                    return;
                }

                // Запрос перевода на русский
                _menu.DisplayMessage("Введите перевод на русском:");
                var meaning = Console.ReadLine()?.Trim();

                // Проверка на пустое значение перевода
                if (string.IsNullOrWhiteSpace(meaning))
                {
                    _menu.DisplayMessage("Перевод не может быть пустым.");
                    return;
                }

                // Запрос уровня
                int level;
                while (true)
                {
                    _menu.DisplayMessage("Введите уровень (1-3):");

                    if (int.TryParse(Console.ReadLine(), out level) && level >= 1 && level <= 3)
                    {
                        break; // Уровень корректный, выходим из цикла
                    }

                    _menu.DisplayMessage("Пожалуйста, введите корректный уровень от 1 до 3.");
                }

                // Создание нового слова
                var word = new Word { Original = original, Meaning = meaning, Level = level };

                // Добавление слова в базу данных
                await _database.AddWordAsync(word);
                _menu.DisplayMessage("Слово успешно добавлено.");
            }
            catch (Exception ex)
            {
                _menu.DisplayMessage($"Произошла ошибка при добавлении слова: {ex.Message}");
            }
        }

        // Метод для валидации слова
        private bool IsValidWord(string word)
        {
            // Проверка: слово должно быть на английском (латиница)
            if (!Regex.IsMatch(word, @"^[A-Za-z]+$"))
                return false;

            // Проверка: слово не должно содержать цифр (они уже исключены, но дополнительная защита)
            if (Regex.IsMatch(word, @"\d"))
                return false;

            // Проверка: слово не должно содержать символов (уже исключено, но дополнительная защита)
            if (Regex.IsMatch(word, @"[^A-Za-z]"))
                return false;

            // Проверка: не должно быть трех одинаковых букв подряд
            if (Regex.IsMatch(word, @"([A-Za-z])\1\1"))
                return false;

            return true;
        }
    }
}
