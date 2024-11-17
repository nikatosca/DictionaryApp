using System.Collections.Generic;
using System.Threading.Tasks;
using dictionary_app.Services;

namespace dictionary_app.Interfaces
{
    public interface IDatabase
    {
        // Метод для добавления нового слова
        Task AddWordAsync(Word word);

        // Метод для удаления слова по его оригиналу
        Task RemoveWordAsync(string term);

        // Метод для получения случайного слова с учетом уровня (взвешенный выбор)
        Task<Word> GetRandomWordAsync();

        // Метод для получения всех слов из базы данных
        Task<List<Word>> GetAllWordsAsync();

        // Метод для подсчета слов с учетом их уровня
        Task<int> CountWordsAsync();

        // Метод для загрузки слов из базы данных (все слова)
        Task<IReadOnlyCollection<Word>> LoadWordsAsync();

        // Метод для сохранения списка слов (перезапись существующих слов)
        Task SaveWordsAsync(List<Word> words);

        // Метод для проверки наличия слова по его оригиналу
        Task<bool> ContainsAsync(string term);
    }
}