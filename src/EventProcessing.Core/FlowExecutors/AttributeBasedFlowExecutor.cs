using EventProcessing.Core.Attributes;
using EventProcessing.Core.CommandCreation;
using EventProcessing.Core.EventStore;
using EventProcessing.Core.StandardEvents;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reactive.Linq;
using System.Linq;

namespace EventProcessing.Core.FlowExecutors
{
    public class AttributeBasedFlowExecutor : IFlowExecutor
    {
        IEventStore eventStore;
        Type flowClass;
        IDictionary<Type, List<EventToCommand>> eventToCommandMapping;
        ICommandRegister commandFactory;
        const string DEFAULT_COMMAND_NAME = "";
        public AttributeBasedFlowExecutor(Type flowClass, IEventStore eventStore, ICommandRegister commandFactory)
        {
            this.eventStore = eventStore;
            this.flowClass = flowClass;
            eventToCommandMapping = new Dictionary<Type, List<EventToCommand>>();
            this.commandFactory = commandFactory;
            eventStore.SubscribeToAllEvents().Subscribe(OnEventRaised);
        }

        private void OnEventRaised(FlowEvent flowEvent)
        {
            if (flowEvent == null)
                return;

            IList<Tuple<string, ICommand>> commands = GetCommands(flowEvent);
            foreach (var command in commands)
            {
                if (!(flowEvent is CommandProcessingEvent))
                    eventStore.AddEvent(new OnStartProcessingCommand(flowEvent.ContextOfEvent, command.Item2));

                var observableEvents = command.Item2.Execute();

                Action onComplete = () =>
                {
                    if (!(flowEvent is CommandProcessingEvent))
                        eventStore.AddEvent(new OnEndProcessingCommand(flowEvent.ContextOfEvent, command.Item2));
                };

                observableEvents.IgnoreElements().Subscribe((e) => { }, onComplete);

                string commandName = command.Item1;// This is needed to capture variable also, we should use command variable in this lambda
                
                observableEvents.Subscribe(currentEvent =>
                {
                    if (currentEvent == null)
                        return;

                    if (currentEvent.ContextOfEvent == null)
                        currentEvent.ContextOfEvent = flowEvent.ContextOfEvent;

                    if (currentEvent.CommandName == DEFAULT_COMMAND_NAME && commandName != DEFAULT_COMMAND_NAME)
                        currentEvent.CommandName = commandName;

                    eventStore.AddEvent(currentEvent);
                }, ex => Console.WriteLine(ex));
            }
        }

        public void Execute(FlowEvent initialEvent)
        {
            var links = flowClass.GetTypeInfo().GetCustomAttributes<LinkEventToCommandAttribute>();
            foreach (var link in links)
            {
                AddEventMapping(link);
            }

            eventStore.AddEvent(initialEvent);
        }

        private IList<Tuple<string, ICommand>> GetCommands(FlowEvent flowEvent)
        {
            var eventType = flowEvent.GetType();
            if (eventToCommandMapping.ContainsKey(eventType))
            {
                IEnumerable<EventToCommand> filteredEventToCommands = GetFilteredEvents(flowEvent.CommandName, eventType);
                return commandFactory.Get(flowEvent.ContextOfEvent, filteredEventToCommands);
            }
            else
            {
                return new List<Tuple<string, ICommand>>();
            }
        }

        private IEnumerable<EventToCommand> GetFilteredEvents(string commandName, Type eventType)
        {
            IEnumerable<EventToCommand> filteredEventToCommands;
            if (commandName != DEFAULT_COMMAND_NAME)
            {
                filteredEventToCommands = eventToCommandMapping[eventType]
                    .Where(ec => ec.SourceEventCommandName == DEFAULT_COMMAND_NAME || ec.SourceEventCommandName == commandName);
            }
            else
            {
                filteredEventToCommands = eventToCommandMapping[eventType]
                    .Where(ec => ec.SourceEventCommandName == DEFAULT_COMMAND_NAME);
            }

            return filteredEventToCommands;
        }

        private void AddEventMapping(LinkEventToCommandAttribute link)
        {
            if (eventToCommandMapping.ContainsKey(link.FlowEvent))
                eventToCommandMapping[link.FlowEvent].Add(link);
            else
                eventToCommandMapping.Add(link.FlowEvent, new List<EventToCommand> { link });
        }
    }
}
