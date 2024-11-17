using System.Threading.Tasks;

namespace dictionary_app.Interfaces
{
    public interface ICommand
    {
        Task Execute();
    }
}