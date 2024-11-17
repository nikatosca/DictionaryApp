namespace dictionary_app.Present;

public class Menu : IMenu
{
    public void Show()
    {
        Console.WriteLine();
        Console.WriteLine("Меню:");
        Console.WriteLine("1. Добавить новое слово в словарь");
        Console.WriteLine("2. Удалить слово из словаря");
        Console.WriteLine("3. Начать тестирование");
        Console.WriteLine("4. Выйти из программы");
        Console.WriteLine();
    }

    // Получает выбор пользователя.
    // Возвращает выбор пользователя в виде строки.
    public string GetUserChoice()
    {
        Console.Write("Выберите действие (1-4): ");
        // Используем оператор null-объединения, чтобы избежать возможного null
        return Console.ReadLine() ?? string.Empty;
    }

    // Отображает сообщение пользователю.
    // "message" - Сообщение для отображения.
    public void DisplayMessage(string message)
    {
        Console.WriteLine(message);
    }

    // Запрашивает ввод от пользователя с отображением подсказки.
    // "message" - Сообщение-подсказка для ввода.
    // Возвращает введённую пользователем строку.
    public string Prompt(string message)
    {
        Console.Write(message);
        // Используем оператор null-объединения, чтобы избежать возможного null
        return Console.ReadLine() ?? string.Empty;
    }
    
}