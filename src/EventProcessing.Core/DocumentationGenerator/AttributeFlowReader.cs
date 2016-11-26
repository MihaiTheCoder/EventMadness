using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using EventProcessing.Core.Attributes;

namespace EventProcessing.Core.DocumentationGenerator
{
    public interface IFLowReader
    {
        FlowModel GetFlow();
    }
    public class AttributeFlowReader
    {
        Type flowClass;
        public AttributeFlowReader(Type flowClass)
        {
            this.flowClass = flowClass;
        }

        public FlatFlow GetFlatFlow()
        {
            var links = flowClass.GetTypeInfo().GetCustomAttributes<LinkEventToCommandAttribute>();
            FlatFlow flatFlow = new FlatFlow();

            var events = links.Select(l => l.FlowEvent).Distinct();
            foreach (var eventFlow in events)
            {
                var commandsLinkedToEvent = links
                    .Where(l => l.FlowEvent == eventFlow)
                    .Select(l => new CommandDescription(l.Command))
                    .ToList();
                flatFlow.EventToCommands.Add(new EventDescription(eventFlow), commandsLinkedToEvent);
            }

            var commands = links.Select(l => l.Command).Distinct();

            foreach (var command in commands)
            {
                var eventsThatCommandMayRaise = command.GetTypeInfo().GetCustomAttributes<MayRaiseAttribute>().ToList();

                flatFlow.CommandToEvents.Add(new CommandDescription(command), eventsThatCommandMayRaise);
            }
            return flatFlow;
        }
    }

    public class FlatFlow
    {
        public Dictionary<EventDescription, List<CommandDescription>> EventToCommands { get; private set; }
        public Dictionary<CommandDescription, List<MayRaiseAttribute>> CommandToEvents { get; private set; }

        public FlatFlow()
        {
            EventToCommands = new Dictionary<EventDescription, List<CommandDescription>>();
            CommandToEvents = new Dictionary<CommandDescription, List<MayRaiseAttribute>>();
        }
    }

    public class CommandDescription
    {
        private Type command;

        public CommandDescription(Type command)
        {
            this.command = command;
            Name = command.Name;
        }

        public string Name { get; set; }

        public override string ToString()
        {
            return "Command " + Name;
        }
    }

    public class EventDescription
    {
        Type eventFlow;
        public EventDescription(Type eventFlow)
        {
            this.eventFlow = eventFlow;
            Name = eventFlow.Name;
        }

        public string Name { get; set; }

        public override string ToString()
        {
            return "Event " + Name;
        }
    }
}
