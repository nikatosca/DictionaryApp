
using dictionary_app.Interfaces;
using dictionary_app.Present;
using dictionary_app.Services;
using dictionary_app.Services.Actions;
using Moq;


namespace dictionary_app.Tests
{
    public class AddNewWordCommandTests
    {
        private readonly Mock<IMenu> _mockMenu;
        private readonly Mock<IDatabase> _mockDatabase;
        private readonly AddNewWordCommand _command;

        public AddNewWordCommandTests()
        {
            _mockMenu = new Mock<IMenu>();
            _mockDatabase = new Mock<IDatabase>();
            _command = new AddNewWordCommand(_mockMenu.Object, _mockDatabase.Object);
        }

        [Fact]
        public async Task Execute_ShouldAddWord_WhenValidEnglishWord()
        {
            // Подготовка Console.ReadLine для последовательного ввода
            using (var consoleInput = new StringReader("hello\nпривет\n1\n"))
            {
                Console.SetIn(consoleInput);

                // Настройка мока меню и базы
                _mockDatabase.Setup(db => db.ContainsAsync("hello")).ReturnsAsync(false);

                // Выполнение команды
                await _command.Execute();

                // Проверка, что метод AddWordAsync был вызван с правильными параметрами
                _mockDatabase.Verify(db => db.AddWordAsync(It.Is<Word>(w => w.Original == "hello" && w.Meaning == "привет")), Times.Once);
                _mockMenu.Verify(menu => menu.DisplayMessage("Слово успешно добавлено."), Times.Once);
            }
        }

        [Fact]
        public async Task Execute_ShouldNotAddWord_WhenWordAlreadyExists()
        {
            // Подготовка Console.ReadLine для последовательного ввода
            using (var consoleInput = new StringReader("hello\nпривет\n1\n"))
            {
                Console.SetIn(consoleInput);

                // Настройка мока базы данных для проверки существующего слова
                _mockDatabase.Setup(db => db.ContainsAsync("hello")).ReturnsAsync(true);

                // Выполнение команды
                await _command.Execute();

                // Проверка, что метод AddWordAsync не был вызван
                _mockDatabase.Verify(db => db.AddWordAsync(It.IsAny<Word>()), Times.Never);
                _mockMenu.Verify(menu => menu.DisplayMessage("Слово 'hello' уже есть в словаре."), Times.Once);
            }
        }

        [Fact]
        public async Task Execute_ShouldNotAddWord_WhenWordIsNotInEnglish()
        {
            using (var consoleInput = new StringReader("привет\nперевод\n1\n"))
            {
                Console.SetIn(consoleInput);

                await _command.Execute();

                _mockDatabase.Verify(db => db.AddWordAsync(It.IsAny<Word>()), Times.Never);
                _mockMenu.Verify(menu => menu.DisplayMessage("Слово должно быть на латинице, без цифр, символов и без трех одинаковых букв подряд."), Times.Once);
            }
        }

        [Fact]
        public async Task Execute_ShouldNotAddWord_WhenWordContainsNumbers()
        {
            using (var consoleInput = new StringReader("hello123\nпривет\n1\n"))
            {
                Console.SetIn(consoleInput);

                await _command.Execute();

                _mockDatabase.Verify(db => db.AddWordAsync(It.IsAny<Word>()), Times.Never);
                _mockMenu.Verify(menu => menu.DisplayMessage("Слово должно быть на латинице, без цифр, символов и без трех одинаковых букв подряд."), Times.Once);
            }
        }

        [Fact]
        public async Task Execute_ShouldNotAddWord_WhenWordContainsThreeIdenticalLettersInARow()
        {
            using (var consoleInput = new StringReader("hellooo\nпривет\n1\n"))
            {
                Console.SetIn(consoleInput);

                await _command.Execute();

                _mockDatabase.Verify(db => db.AddWordAsync(It.IsAny<Word>()), Times.Never);
                _mockMenu.Verify(menu => menu.DisplayMessage("Слово должно быть на латинице, без цифр, символов и без трех одинаковых букв подряд."), Times.Once);
            }
        }

        [Fact]
        public async Task Execute_ShouldNotAddWord_WhenWordIsEmpty()
        {
            using (var consoleInput = new StringReader("\n"))
            {
                Console.SetIn(consoleInput);

                await _command.Execute();

                _mockDatabase.Verify(db => db.AddWordAsync(It.IsAny<Word>()), Times.Never);
                _mockMenu.Verify(menu => menu.DisplayMessage("Слово не может быть пустым."), Times.Once);
            }
        }
    }
}
