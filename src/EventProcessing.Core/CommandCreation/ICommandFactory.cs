using EventProcessing.Core.EventStore;
using System;
using System.Collections.Generic;

namespace EventProcessing.Core.CommandCreation
{
    public interface ICommandFactory
    {
        void RegisterCommand<TCommand>(Func<FlowContext,TCommand> commandCreation) where TCommand : ICommand;

        ICommand Get(FlowContext context, Type commandType);

        IList<ICommand> Get(FlowContext context, IList<Type> commands);
    }
}
