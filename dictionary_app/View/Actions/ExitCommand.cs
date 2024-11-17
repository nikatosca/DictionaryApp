using dictionary_app.Interfaces;
using System;
using System.Threading.Tasks;

namespace dictionary_app.Services.Actions
{
    public class ExitCommand : ICommand
    {
        private readonly Action _exitAction;

        public ExitCommand(Action exitAction)
        {
            _exitAction = exitAction;
        }

        public Task Execute()
        {
            _exitAction.Invoke();
            return Task.CompletedTask;
        }
    }
}