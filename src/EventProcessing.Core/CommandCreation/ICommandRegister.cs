using EventProcessing.Core.Attributes;
using EventProcessing.Core.EventStore;
using System;
using System.Collections.Generic;

namespace EventProcessing.Core.CommandCreation
{
    public interface ICommandRegister
    {
        void RegisterCommand<TCommand>(Func<FlowContext,TCommand> commandCreation, string commandName = "") where TCommand : ICommand;

        Tuple<string, ICommand> Get(FlowContext flwoContext, EventToCommand eventToCommand);

        IList<Tuple<string, ICommand>> Get(FlowContext flwoContext, IEnumerable<EventToCommand> eventToCommand);
    }
}
