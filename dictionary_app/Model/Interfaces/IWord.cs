namespace dictionary_app.Interfaces;

public abstract class IWord
{
    public abstract int Id { get; set; }
    public abstract string Original { get; }
    public abstract string Meaning { get; }
    public abstract int Level { get; set; }
}