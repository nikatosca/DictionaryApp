using dictionary_app.Interfaces;
using dictionary_app.Present;
using dictionary_app.Services.Actions;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace dictionary_app.View
{
    public class CommandFactory
    {
        private readonly IMenu _menu;
        private readonly IDatabase _database;
        private readonly Action _exitAction;

        public CommandFactory(IMenu menu, IDatabase database)
        {
            _menu = menu;
            _database = database;
            _exitAction = ExitAction;
        }

        private void ExitAction()
        {
            Environment.Exit(0);
        }

        public ICommand GetCommand(string choice)
        {
            var commandMapping = new Dictionary<string, Func<ICommand>>()
            {
                { "1", () => new AddNewWordCommand(_menu, _database) },
                { "2", () => new RemoveWordCommand(_menu, _database) },
                { "3", () => new StartQuizCommand(_menu, _database) },
                { "4", () => new ExitCommand(_exitAction) }
            };

            return commandMapping.TryGetValue(choice, out var commandFactory) ? commandFactory() : new InvalidCommand(_menu);
        }

        public async Task Start()
        {
            var exit = false;

            while (!exit)
            {
                _menu.DisplayMessage("Добро пожаловать в программу для изучения иностранных слов!");
                _menu.Show();
                string choice = _menu.GetUserChoice();
                ICommand command = GetCommand(choice);
                await command.Execute();

                if (command is ExitCommand)
                {
                    exit = true;
                }
            }
        }
    }
}