using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using dictionary_app.Interfaces;
using dictionary_app.Services;
using dictionary_app.Present;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace dictionary_app.Tests
{
    public class DatabaseTests
    {
        private readonly Mock<AppDbContext> _mockContext;
        private readonly Mock<DbSet<Word>> _mockDbSet;
        private readonly Database _database;

        public DatabaseTests()
        {
            // Мокируем DbContext и DbSet
            _mockContext = new Mock<AppDbContext>();
            _mockDbSet = new Mock<DbSet<Word>>();

            // Настройка контекста для возврата мокированного DbSet
            _mockContext.Setup(c => c.Words).Returns(_mockDbSet.Object);
            _database = new Database(_mockContext.Object);
        }

        [Fact]
        public async Task AddWordAsync_ShouldAddWordToDatabase()
        {
            // Arrange
            var word = new Word { Original = "test", Meaning = "тест", Level = 1 };

            // Мокируем асинхронное добавление в DbSet
            _mockDbSet.Setup(dbSet => dbSet.AddAsync(It.IsAny<Word>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Word w, CancellationToken token) =>
                {
                    // Возвращаем мокированный EntityEntry
                    var entityEntryMock = new Mock<EntityEntry<Word>>();
                    entityEntryMock.Setup(entry => entry.Entity).Returns(w);
                    return new ValueTask<EntityEntry<Word>>(entityEntryMock.Object);
                });

            _mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            await _database.AddWordAsync(word);

            // Assert
            _mockDbSet.Verify(dbSet => dbSet.AddAsync(It.Is<Word>(w => w == word), It.IsAny<CancellationToken>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        
        
        [Fact]
        public async Task RemoveWordAsync_ShouldRemoveWordFromDatabase()
        {
            // Arrange
            var word = new Word { Original = "test", Meaning = "тест", Level = 1 };
            var words = new List<Word> { word };

            // Мокируем возврат найденных слов
            var queryable = words.AsQueryable();
            _mockDbSet.As<IQueryable<Word>>().Setup(m => m.Provider).Returns(queryable.Provider);
            _mockDbSet.As<IQueryable<Word>>().Setup(m => m.Expression).Returns(queryable.Expression);
            _mockDbSet.As<IQueryable<Word>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            _mockDbSet.As<IQueryable<Word>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            _mockDbSet.Setup(dbSet => dbSet.RemoveRange(It.IsAny<IEnumerable<Word>>()))
                .Verifiable();

            _mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            await _database.RemoveWordAsync("test");

            // Assert
            _mockDbSet.Verify(dbSet => dbSet.RemoveRange(It.Is<IEnumerable<Word>>(words => words.Contains(word))),
                Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RemoveWordAsync_ShouldThrowException_WhenWordNotFound()
        {
            // Arrange
            var words = new List<Word>(); // Нет слов

            var queryable = words.AsQueryable();
            _mockDbSet.As<IQueryable<Word>>().Setup(m => m.Provider).Returns(queryable.Provider);
            _mockDbSet.As<IQueryable<Word>>().Setup(m => m.Expression).Returns(queryable.Expression);
            _mockDbSet.As<IQueryable<Word>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            _mockDbSet.As<IQueryable<Word>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            // Act & Assert
            var exception =
                await Assert.ThrowsAsync<InvalidOperationException>(() => _database.RemoveWordAsync("test"));
            Assert.Equal("Слово 'test' не найдено в базе данных.", exception.Message);
        }

        [Fact]
        public async Task GetRandomWordAsync_ShouldReturnRandomWord()
        {
            // Arrange
            var words = new List<Word>
            {
                new Word { Original = "test1", Meaning = "тест1", Level = 1 },
                new Word { Original = "test2", Meaning = "тест2", Level = 2 }
            };

            _mockDbSet.Setup(dbSet => dbSet.ToListAsync(It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(words);

            // Act
            var randomWord = await _database.GetRandomWordAsync();

            // Assert
            Assert.NotNull(randomWord);
            Assert.Contains(randomWord.Original, new[] { "test1", "test2" });
        }

        [Fact]
        public async Task GetAllWordsAsync_ShouldReturnAllWords()
        {
            // Arrange
            var words = new List<Word>
            {
                new Word { Original = "test1", Meaning = "тест1", Level = 1 },
                new Word { Original = "test2", Meaning = "тест2", Level = 2 }
            };

            _mockDbSet.Setup(dbSet => dbSet.ToListAsync(It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(words);

            // Act
            var result = await _database.GetAllWordsAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, w => w.Original == "test1");
            Assert.Contains(result, w => w.Original == "test2");
        }

        [Fact]
        public async Task CountWordsAsync_ShouldReturnCorrectCountBasedOnLevel()
        {
            // Arrange
            var words = new List<Word>
            {
                new Word { Original = "test1", Meaning = "тест1", Level = 1 },
                new Word { Original = "test2", Meaning = "тест2", Level = 2 },
                new Word { Original = "test3", Meaning = "тест3", Level = 3 }
            };

            _mockDbSet.Setup(dbSet => dbSet.ToListAsync(It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(words);

            // Act
            var count = await _database.CountWordsAsync();

            // Assert
            Assert.Equal(6, count); // word1 = 3, word2 = 2, word3 = 1
        }

        [Fact]
        public async Task SaveWordsAsync_ShouldUpdateExistingWords()
        {
            // Arrange
            var word = new Word { Original = "test", Meaning = "тест", Level = 1 };
            await _database.AddWordAsync(word);

            word.Level = 2;
            var wordsToUpdate = new List<Word> { word };

            _mockDbSet.Setup(dbSet => dbSet.Update(It.IsAny<Word>()))
                .Verifiable();

            _mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            await _database.SaveWordsAsync(wordsToUpdate);

            // Assert
            _mockDbSet.Verify(dbSet => dbSet.Update(It.Is<Word>(w => w.Level == 2)), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ContainsAsync_ShouldReturnTrue_WhenWordExists()
        {
            // Arrange
            var word = new Word { Original = "test", Meaning = "тест", Level = 1 };
            var words = new List<Word> { word };

            var queryable = words.AsQueryable();
            _mockDbSet.As<IQueryable<Word>>().Setup(m => m.Provider).Returns(queryable.Provider);
            _mockDbSet.As<IQueryable<Word>>().Setup(m => m.Expression).Returns(queryable.Expression);
            _mockDbSet.As<IQueryable<Word>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            _mockDbSet.As<IQueryable<Word>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            // Act
            var exists = await _database.ContainsAsync("test");

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task ContainsAsync_ShouldReturnFalse_WhenWordDoesNotExist()
        {
            // Arrange
            var words = new List<Word>();

            var queryable = words.AsQueryable();
            _mockDbSet.As<IQueryable<Word>>().Setup(m => m.Provider).Returns(queryable.Provider);
        }

    }
}

