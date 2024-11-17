namespace dictionary_app.View;

public class Exceptions
{
    private string message;
    public Exceptions(string e)
    {
        message = e;
        Console.WriteLine(message);
    }
}