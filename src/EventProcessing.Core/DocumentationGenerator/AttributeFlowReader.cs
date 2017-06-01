using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using EventProcessing.Core.Attributes;
using EventProcessing.Core.StandardEvents;

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
            var links = flowClass
                .GetTypeInfo()
                .GetCustomAttributes<LinkEventToCommandAttribute>()
                .ToArray();

            FlatFlow flatFlow = new FlatFlow();

            var events = links
                .Select(l => new EventDescription(l.FlowEvent, l.SourceEventCommandName))
                .Distinct();

            foreach (var eventFlow in events)
            {
                var commandsLinkedToEvent = links
                    .Where(l => l.FlowEvent == eventFlow.FlowEvent && l.SourceEventCommandName == eventFlow.SourceCommandName)
                    .Select(l => new CommandDescription(l.Command, l.CommandName))
                    .ToList();
                flatFlow.EventToCommands.Add(new EventDescription(eventFlow.FlowEvent, eventFlow.SourceCommandName), commandsLinkedToEvent);
            }

            var commands = links.Select(l => new CommandDescription(l.Command, l.CommandName)).Distinct();

            foreach (var command in commands)
            {
                var eventsThatCommandMayRaise = command.Command.GetTypeInfo().GetCustomAttributes<MayRaiseAttribute>().ToList();

                flatFlow.CommandToEvents.Add(command, eventsThatCommandMayRaise);
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

    public class ActivityDiagramBuilder
    {
        public List<Transition> GetActivityDiagram(FlatFlow flatFlow, bool everyEventHasAnIssuer)
        {
            List<Transition> allTransitions = new List<Transition>();
            CommandDescription initialCommand = new CommandDescription(null, "Start of flow");

            var initialEvent = flatFlow.EventToCommands.First(e => e.Key.FlowEvent == typeof(OnStart)).Key;
            allTransitions.AddRange(GetTransitions(flatFlow, new SourceOfEvent(initialCommand, null), initialEvent));

            var restOfTheEvents = flatFlow.EventToCommands
                .Where(eventToCommand => eventToCommand.Key != initialEvent)
                .Select(eventToCommand => eventToCommand.Key)
                .Distinct();

            foreach (var flowEvent in restOfTheEvents)
            {
                IEnumerable<SourceOfEvent> sourcesOfEvents;
                sourcesOfEvents = GetSourceOfEvents(flatFlow, flowEvent, everyEventHasAnIssuer);

                foreach (var sourceOfEvent in sourcesOfEvents)
                {
                    allTransitions.AddRange(GetTransitions(flatFlow, sourceOfEvent, flowEvent));
                }
            }

            return allTransitions;
        }

        private static IEnumerable<SourceOfEvent> GetSourceOfEvents(FlatFlow flatFlow, EventDescription flowEvent, bool everyEventHasAnIssuer)
        {
            IEnumerable<SourceOfEvent> commands = commands = flatFlow.CommandToEvents
                .Where(c => c.Value.Any(mayRaise => mayRaise.EventThatMayBeRaised == flowEvent.FlowEvent))
                .Select(c => new SourceOfEvent(c.Key, c.Value.First(mayRaise => mayRaise.EventThatMayBeRaised == flowEvent.FlowEvent).Condition));                

            if (everyEventHasAnIssuer && !commands.Any())
                throw new InvalidProgramException($"There is no command attached to event:{flowEvent}");

            return commands;
        }

        private static List<Transition> GetTransitions(FlatFlow flatFlow, SourceOfEvent sourceCommand, EventDescription flowEvent)
        {
            List<Transition> transitions = new List<Transition>();
            var startEvent = flatFlow.EventToCommands.First(e => e.Key == flowEvent);
            foreach (var transition in startEvent.Value)
            {
                transitions.Add(new Transition(startEvent.Key, sourceCommand.Command, transition, sourceCommand.ConditionOfRaise));
            }

            return transitions;
        }

        private static void NewMethod(CommandDescription commandDescription, List<Transition> transitions, KeyValuePair<EventDescription, List<CommandDescription>> startEvent)
        {
            
        }
    }

    public class Node
    {
        public string Name { get; set; }        

        public Node(string name)
        {
            Name = name;
        }
    }

    public class SourceOfEvent
    {
        public SourceOfEvent(CommandDescription command, string conditionOfRaise)
        {
            Command = command;
            ConditionOfRaise = conditionOfRaise;
        }

        public CommandDescription Command { get; set; }

        public string ConditionOfRaise { get; set; }
    }

    public class Transition
    {
        public Transition(EventDescription transitionEvent, CommandDescription sourceCommand, CommandDescription targetCommand, string conditionForTransition)
        {
            TransitionEvent = transitionEvent;
            SourceCommand = sourceCommand;
            TargetCommand = targetCommand;
            ConditionForTransition = conditionForTransition;
        }

        public EventDescription TransitionEvent { get; set; }

        public CommandDescription SourceCommand { get; set; }

        public CommandDescription TargetCommand { get; set; }

        public string ConditionForTransition { get; set; }
    }

    public class CommandDescription
    {
        public Type Command { get; set; }
        public string CommandName { get; set; }

        public CommandDescription(Type command, string commandName)
        {
            Command = command;
            CommandName = commandName;
        }        

        public override string ToString()
        {
            string name =  "Command: " + Command.Name;

            if (!string.IsNullOrWhiteSpace(CommandName))
                name += "; name: " + CommandName;

            return name;
        }
    }

    public class EventDescription
    {
        public Type FlowEvent { get; set; }
        public string SourceCommandName { get; set; }
        public EventDescription(Type flowEvent, string sourceCommandName)
        {
            FlowEvent = flowEvent;
            SourceCommandName = sourceCommandName;

        }

        public override string ToString()
        {
            return $"Event: {FlowEvent.Name}, SourceCommandName:{SourceCommandName}";
        }
    }
}
