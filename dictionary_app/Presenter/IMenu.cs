namespace dictionary_app.Present;

public interface IMenu
{
    // Отображает меню пользователю.
    void Show();

    // Получает выбор пользователя.
    // Возвращает строку, представляющую выбор пользователя.
    string GetUserChoice();

    // Отображает сообщение пользователю.
    // "message" - Сообщение для отображения.
    void DisplayMessage(string message);

    // Запрашивает ввод от пользователя.
    // "message" - Сообщение-подсказка для ввода.
    // Возвращает строку, введенную пользователем.
    string Prompt(string message);
}