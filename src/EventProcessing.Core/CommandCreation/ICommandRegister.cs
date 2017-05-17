using EventProcessing.Core.EventStore;
using System;
using System.Collections.Generic;

namespace EventProcessing.Core.CommandCreation
{
    public interface ICommandRegister
    {
        void RegisterCommand<TCommand>(Func<FlowContext,TCommand> commandCreation, string stepName = "") where TCommand : ICommand;

        IList<Tuple<string, ICommand>> Get(FlowContext context, Type commandType, string stepName);

        IList<Tuple<string, ICommand>> Get(FlowContext context, IList<Type> commands, string stepName);
    }
}
