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
    public class CommandFactory : ICommandFactory
    {
        private IoCResolver iocResolver;
        private IEventStore eventStore;
        IDictionary<Type, Func<FlowContext, ICommand>> commandCreationDictionary;

        public CommandFactory(IoCResolver iocResolver, IEventStore eventStore)
        {
            this.iocResolver = iocResolver;
            this.eventStore = eventStore;
            commandCreationDictionary = new Dictionary<Type, Func<FlowContext, ICommand>>();
        }

        public ICommand Get(FlowContext context, Type commandType)
        {
            if (!commandCreationDictionary.ContainsKey(commandType))
                throw new ArgumentException("Command:{0} not registered", commandType.FullName);

            return commandCreationDictionary[commandType].Invoke(context);
        }

        public void RegisterCommand(Type commandClass, Func<FlowContext, ICommand> commandCreation)
        {
            commandCreationDictionary.Add(commandClass, commandCreation);
        }
    }
}
