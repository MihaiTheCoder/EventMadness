using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventProcessing.Core;
using EventProcessing.Core.EventStore;
using System.Reflection;
using EventProcessing.Core.Helpers;

namespace EventProcessing.Core.CommandCreation
{
    public class CommandRegister : ICommandRegister
    {
        private IEventStore eventStore;
        const string defaultStepName = "";
        IDictionary<Type, Dictionary<string, Func<FlowContext, ICommand>>> commandCreationDictionary;

        public CommandRegister(IEventStore eventStore)
        {
            this.eventStore = eventStore;
            commandCreationDictionary = new Dictionary<Type, Dictionary<string, Func<FlowContext, ICommand>>>();
        }

        public IList<Tuple<string, ICommand>> Get(FlowContext context, Type commandType, string stepName)
        {
            if (!commandCreationDictionary.ContainsKey(commandType))
                throw new ArgumentException("Command:{0} not registered", commandType.FullName);

            var commandsOfType = commandCreationDictionary[commandType];

            List<Tuple<string, ICommand>> commands = new List<Tuple<string, ICommand>>();
            if (commandsOfType.ContainsKey(stepName))
            {
                commands.Add(new Tuple<string, ICommand>(stepName, commandsOfType[stepName].Invoke(context)));
            }
            if (stepName != defaultStepName && commandsOfType.ContainsKey(defaultStepName))
            {
                commands.Add(new Tuple<string, ICommand>(defaultStepName, commandsOfType[defaultStepName].Invoke(context)));
            }
            return commands;
        }

        public void RegisterCommand<TCommand>(Func<FlowContext, TCommand> commandCreation, string stepName) where TCommand : ICommand
        {
            var commandType = typeof(TCommand);
            if (commandCreationDictionary.ContainsKey(commandType))
            {
                commandCreationDictionary[commandType].Add(stepName, (context) => commandCreation(context));
            }
            else
            {
                commandCreationDictionary.Add(typeof(TCommand), new Dictionary<string, Func<FlowContext, ICommand>>
                    { { stepName, (context) => commandCreation(context) } });
            }

        }

        public IList<Tuple<string,ICommand>> Get(FlowContext context, IList<Type> commands, string stepName)
        {
            List<Tuple<string, ICommand>> commandInstances = new List<Tuple<string, ICommand>>();
            foreach (var command in commands)
            {
                commandInstances.AddRange(Get(context, command, stepName));
            }
            return commandInstances;
        }
    }
}
