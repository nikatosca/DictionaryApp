using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using dictionary_app.Interfaces;
using dictionary_app.Present;
using dictionary_app.Services;
using dictionary_app.Services.Actions;

namespace dictionary_app.Tests
{
    public class StartQuizCommandTests
    {
        private readonly Mock<IMenu> _mockMenu;
        private readonly Mock<IDatabase> _mockDatabase;
        private readonly StartQuizCommand _command;

        public StartQuizCommandTests()
        {
            _mockMenu = new Mock<IMenu>();
            _mockDatabase = new Mock<IDatabase>();
            _command = new StartQuizCommand(_mockMenu.Object, _mockDatabase.Object);
        }

        [Fact]
        public async Task Execute_ShouldDisplayMessage_WhenNoWordsInDatabase()
        {
            // Arrange
            _mockDatabase.Setup(db => db.GetAllWordsAsync()).ReturnsAsync(new List<Word>());

            // Act
            await _command.Execute();

            // Assert
            _mockMenu.Verify(menu => menu.DisplayMessage("Словарь пуст. Добавьте слова для начала тестирования."), Times.Once);
        }

        [Fact]
        public async Task Execute_ShouldDisplayMessage_WhenNoRandomWordAvailable()
        {
            // Arrange
            _mockDatabase.Setup(db => db.GetAllWordsAsync()).ReturnsAsync(new List<Word> { new Word { Original = "test", Meaning = "тест" } });
            _mockDatabase.Setup(db => db.GetRandomWordAsync()).ReturnsAsync((Word)null);

            // Act
            await _command.Execute();

            // Assert
            _mockMenu.Verify(menu => menu.DisplayMessage("Словарь пуст. Добавьте слова для продолжения тестирования."), Times.Once);
        }

        [Fact]
        public async Task Execute_ShouldPromptUserForAnswer()
        {
            // Arrange
            var word = new Word { Original = "test", Meaning = "тест" };
            _mockDatabase.Setup(db => db.GetAllWordsAsync()).ReturnsAsync(new List<Word> { word });
            _mockDatabase.Setup(db => db.GetRandomWordAsync()).ReturnsAsync(word);
            _mockMenu.Setup(menu => menu.Prompt(It.IsAny<string>())).Returns("тест");

            // Act
            await _command.Execute();

            // Assert
            _mockMenu.Verify(menu => menu.Prompt($"Переведите слово: {word.Original}\nВаш ответ: "), Times.Once);
            _mockMenu.Verify(menu => menu.DisplayMessage("Правильно!"), Times.Once);
        }

        [Fact]
        public async Task Execute_ShouldExitWhenUserTypesExit()
        {
            // Arrange
            var word = new Word { Original = "test", Meaning = "тест" };
            _mockDatabase.Setup(db => db.GetAllWordsAsync()).ReturnsAsync(new List<Word> { word });
            _mockDatabase.Setup(db => db.GetRandomWordAsync()).ReturnsAsync(word);
            _mockMenu.Setup(menu => menu.Prompt(It.IsAny<string>())).Returns("exit");

            // Act
            await _command.Execute();

            // Assert
            _mockMenu.Verify(menu => menu.Prompt(It.IsAny<string>()), Times.Once);
            _mockMenu.Verify(menu => menu.DisplayMessage(It.IsAny<string>()), Times.Once);
            _mockDatabase.Verify(db => db.SaveWordsAsync(It.IsAny<List<Word>>()), Times.Never);
        }

        [Fact]
        public async Task Execute_ShouldUpdateWordLevel_WhenAnswerIsCorrect()
        {
            // Arrange
            var word = new Word { Original = "test", Meaning = "тест", Level = 1 };
            _mockDatabase.Setup(db => db.GetAllWordsAsync()).ReturnsAsync(new List<Word> { word });
            _mockDatabase.Setup(db => db.GetRandomWordAsync()).ReturnsAsync(word);
            _mockMenu.Setup(menu => menu.Prompt(It.IsAny<string>())).Returns("тест");

            // Act
            await _command.Execute();

            // Assert
            _mockDatabase.Verify(db => db.SaveWordsAsync(It.Is<List<Word>>(words => words[0].Level == 2)), Times.Once);
        }

        [Fact]
        public async Task Execute_ShouldDecreaseWordLevel_WhenAnswerIsIncorrect()
        {
            // Arrange
            var word = new Word { Original = "test", Meaning = "тест", Level = 2 };
            _mockDatabase.Setup(db => db.GetAllWordsAsync()).ReturnsAsync(new List<Word> { word });
            _mockDatabase.Setup(db => db.GetRandomWordAsync()).ReturnsAsync(word);
            _mockMenu.Setup(menu => menu.Prompt(It.IsAny<string>())).Returns("ошибка");

            // Act
            await _command.Execute();

            // Assert
            _mockDatabase.Verify(db => db.SaveWordsAsync(It.Is<List<Word>>(words => words[0].Level == 1)), Times.Once);
        }
    }
}
