using EventProcessing.Core.EventStore;
using System;

namespace EventProcessing.Core.CommandCreation
{
    public interface ICommandFactory
    {
        void RegisterCommand(Type commandClass, Func<FlowContext,ICommand> commandCreation);

        ICommand Get(FlowContext context, Type commandType);
    }
}
