using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventProcessing.Core;
using EventProcessing.Core.EventStore;
using System.Reflection;
using EventProcessing.Core.Helpers;
using EventProcessing.Core.Attributes;

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

        public Tuple<string, ICommand> Get(FlowContext flowEvent, EventToCommand eventToCommand)
        {
            if (!commandCreationDictionary.ContainsKey(eventToCommand.Command))
                throw new ArgumentException("Command:{0} not registered", eventToCommand.Command.FullName);

            var commandsOfType = commandCreationDictionary[eventToCommand.Command];

            if (!commandsOfType.ContainsKey(eventToCommand.CommandName))
                throw new ArgumentException("Command:{0} not registered with the expected name", eventToCommand.CommandName);

            return new Tuple<string, ICommand>(eventToCommand.CommandName, commandsOfType[eventToCommand.CommandName].Invoke(flowEvent));
        }

        public IList<Tuple<string,ICommand>> Get(FlowContext flowEvent, IEnumerable<EventToCommand> eventToCommands)
        {
            List<Tuple<string, ICommand>> commandInstances = new List<Tuple<string, ICommand>>();
            foreach (var eventToCommand in eventToCommands)
            {
                commandInstances.Add(Get(flowEvent, eventToCommand));
            }
            return commandInstances;
        }
    }
}
