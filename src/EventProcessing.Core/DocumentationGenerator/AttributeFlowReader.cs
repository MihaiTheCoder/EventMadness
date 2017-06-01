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

            
            var commands = links                
                .ToDictionary(link => link, l => new CommandDescription(l.Command, l.CommandName));

            var distinctCommands = commands
                .GroupBy(e => new { e.Value.Command, e.Value.CommandName })
                .Select(g => g.First().Value);


            var events = links                
                .ToDictionary(link => link, link => new EventDescription(link.FlowEvent, link.SourceEventCommandName));

            var distinctEvents = events
                .GroupBy(e => new { e.Value.FlowEvent, e.Value.SourceCommandName })
                .Select(g => g.First().Value);


            foreach (var eventFlow in distinctEvents)
            {
                var commandsLinkedToEvent = links
                    .Where(link => link.FlowEvent == eventFlow.FlowEvent && link.SourceEventCommandName == eventFlow.SourceCommandName)
                    .Select(link => commands[link])
                    .ToList();

                flatFlow.EventToCommands.Add(eventFlow, commandsLinkedToEvent);
            }            

            foreach (var command in distinctCommands)
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

        private static IEnumerable<SourceOfEvent> GetSourceOfEvents(
            FlatFlow flatFlow, 
            EventDescription flowEvent, 
            bool everyEventHasAnIssuer)
        {
            IEnumerable<SourceOfEvent> commands = commands = flatFlow.CommandToEvents
                .Where(c => c.Value.Any(mayRaise => mayRaise.EventThatMayBeRaised == flowEvent.FlowEvent))
                .Select(c => new SourceOfEvent(c.Key, c.Value.First(mayRaise => mayRaise.EventThatMayBeRaised == flowEvent.FlowEvent)));                

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
    }

    public class SourceOfEvent
    {
        public SourceOfEvent(CommandDescription command, MayRaiseAttribute mayRaiseAttribute)
        {
            Command = command;

            if (mayRaiseAttribute != null)
            {
                ConditionOfRaise = mayRaiseAttribute.Condition;
                ExtraMessage = mayRaiseAttribute.ExtraMessage;
            }
        }

        public CommandDescription Command { get; set; }

        public string ConditionOfRaise { get; set; }

        public string ExtraMessage { get; set; }
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

        public override string ToString()
        {
            return $"Source:{SourceCommand}, Target:{TargetCommand}, Transition:{TransitionEvent}";            
        }
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
            string name = "Command: ";
            if (Command != null && !string.IsNullOrEmpty(CommandName))
            {
                name += $"{Command.Name}, name: {CommandName}";
            }
            else if (Command != null)
            {
                name += $"{Command.Name}";
            }
            else if (!string.IsNullOrEmpty(CommandName))
            {
                name += $"{CommandName}";
            }

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
