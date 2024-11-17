using Microsoft.EntityFrameworkCore;
using dictionary_app.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dictionary_app.Services
{
    public class Database : IDatabase
    {
        private readonly AppDbContext _context;

        public Database(AppDbContext context)
        {
            _context = context;
        }

        // Метод для добавления слова
        public async Task AddWordAsync(Word word)
        {
            // Используем транзакцию для атомарности операции
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Добавляем новое слово
                    await _context.Words.AddAsync(word);

                    // Сохраняем изменения в базе данных
                    await _context.SaveChangesAsync();

                    // Завершаем транзакцию (подтверждаем изменения)
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    // В случае ошибки откатываем транзакцию
                    await transaction.RollbackAsync();

                    // Логируем ошибку или пробрасываем ее дальше
                    throw new InvalidOperationException("Ошибка при добавлении слова в базу данных", ex);
                }
            }
        }

        // Метод для удаления слова по его оригиналу
        public async Task RemoveWordAsync(string term)
        {
            var wordsToRemove = await _context.Words
                .Where(w => w.Original.ToLower() == term.ToLower())
                .ToListAsync();

            if (wordsToRemove.Any())
            {
                _context.Words.RemoveRange(wordsToRemove);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException($"Слово '{term}' не найдено в базе данных.");
            }
        }

        // Метод для получения случайного слова, с учетом уровня (взвешенный выбор)
        public async Task<Word> GetRandomWordAsync()
        {
            var allWords = await _context.Words.ToListAsync();

            if (!allWords.Any())
            {
                return null;
            }

            var weightedList = new List<Word>();
            foreach (var word in allWords)
            {
                int repetitions = word.Level == 1 ? 3 : word.Level == 2 ? 2 : 1;
                for (int i = 0; i < repetitions; i++)
                {
                    weightedList.Add(word);
                }
            }

            var random = new Random();
            return weightedList[random.Next(weightedList.Count)];
        }

        // Метод для получения всех слов
        public async Task<List<Word>> GetAllWordsAsync()
        {
            return await _context.Words.ToListAsync();
        }

        // Метод для подсчета слов с учетом их уровня
        public async Task<int> CountWordsAsync()
        {
            var words = await _context.Words.ToListAsync();
            return words.Sum(w => w.Level == 3 ? 1 : (w.Level == 2 ? 2 : 3));
        }

        // Метод для загрузки слов из базы данных
        public async Task<IReadOnlyCollection<Word>> LoadWordsAsync()
        {
            return await _context.Words.ToListAsync();
        }

        // Метод для сохранения слов (перезапись списка слов)
        public async Task SaveWordsAsync(List<Word> words)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (var word in words)
                    {
                        var existingWord = await _context.Words.FirstOrDefaultAsync(w => w.Id == word.Id);
                        if (existingWord != null)
                        {
                            _context.Entry(existingWord).CurrentValues.SetValues(word);
                        }
                        else
                        {
                            await _context.Words.AddAsync(word);
                        }
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new InvalidOperationException("Ошибка при сохранении слов", ex);
                }
            }
        }

        // Проверка на наличие слова по его оригиналу
        public async Task<bool> ContainsAsync(string term)
        {
            return await _context.Words
                .AnyAsync(word => word.Original.ToLower() == term.ToLower());
        }
    }
}
