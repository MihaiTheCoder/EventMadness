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
        IDictionary<Type, Func<FlowContext, ICommand>> commandCreationDictionary;

        public CommandRegister(IEventStore eventStore)
        {
            this.eventStore = eventStore;
            commandCreationDictionary = new Dictionary<Type, Func<FlowContext, ICommand>>();
        }

        public ICommand Get(FlowContext context, Type commandType)
        {
            if (!commandCreationDictionary.ContainsKey(commandType))
                throw new ArgumentException("Command:{0} not registered", commandType.FullName);

            return commandCreationDictionary[commandType].Invoke(context);
        }

        public void RegisterCommand<TCommand>(Func<FlowContext, TCommand> commandCreation) where TCommand : ICommand
        {
            commandCreationDictionary.Add(typeof(TCommand), (context) => commandCreation(context));
        }

        public IList<ICommand> Get(FlowContext context, IList<Type> commands)
        {
            List<ICommand> commandInstances = new List<ICommand>();
            foreach (var command in commands)
            {
                commandInstances.Add(Get(context, command));
            }
            return commandInstances;
        }
    }
}
