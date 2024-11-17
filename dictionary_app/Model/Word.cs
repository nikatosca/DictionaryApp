using dictionary_app.Interfaces;

namespace dictionary_app.Services;
public class Word
{
    public int Id { get; set; }
    public string Original { get; set; } = string.Empty;
    public string Meaning { get; set; } = string.Empty;
    public int Level { get; set; }

    // Конструктор по умолчанию
    public Word() { }

    // Параметризированный конструктор
    public Word(string original, string meaning, int level)
    {
        Original = original;
        Meaning = meaning;
        Level = level;
    }
    
}
