using EventProcessing.Core.EventStore;
using EventProcessing.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventProcessing.Core.Attributes
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public sealed class LinkEventToCommandAttribute : Attribute, EventToCommand
    {
        static Type flowEventType = typeof(FlowEvent);
        static Type commandInterface = typeof(ICommand);
        
       
        public LinkEventToCommandAttribute(Type flowEvent, Type command, string sourceEventCommandName = "", string commandName = "")
        {
            FlowEvent = flowEvent;

            Command = command;

            if (!FlowEvent.IsSameOrChildOf(flowEventType))
                throw new ArgumentException(string.Format("Class {0} must inherit from: {1}", flowEvent.FullName, flowEventType.FullName));

            if (!Command.ImplementsInterface(commandInterface))
                throw new ArgumentException(string.Format("Class {0} must implement {1}", command.FullName, commandInterface.FullName));

            SourceEventCommandName = sourceEventCommandName;

            CommandName = commandName;
        }

        public Type FlowEvent { get; private set; }

        public Type Command { get; private set; }

        // This is a named argument
        public string FlowIdentifier { get; set; }

        public string SourceEventCommandName { get; set; }

        public string CommandName { get; set; }

        public override string ToString()
        {
            return $"LinkEventToCommand:FlowEvent:{FlowEvent.Name}, Command: {Command.Name}, SourceEventCommandName: {SourceEventCommandName}, CommandName: {CommandName}";            
        }
    }
}
