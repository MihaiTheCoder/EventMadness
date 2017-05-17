using EventProcessing.Core.Attributes;
using EventProcessing.Core.CommandCreation;
using EventProcessing.Core.EventStore;
using EventProcessing.Core.StandardEvents;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reactive.Linq;

namespace EventProcessing.Core.FlowExecutors
{
    public class AttributeBasedFlowExecutor : IFlowExecutor
    {
        IEventStore eventStore;
        Type flowClass;
        IDictionary<Type, List<Type>> eventToCommandMapping;
        ICommandRegister commandFactory;
        public AttributeBasedFlowExecutor(Type flowClass, IEventStore eventStore, ICommandRegister commandFactory)
        {
            this.eventStore = eventStore;
            this.flowClass = flowClass;
            eventToCommandMapping = new Dictionary<Type, List<Type>>();
            this.commandFactory = commandFactory;
            eventStore.SubscribeToAllEvents().Subscribe(OnEventRaised);
        }

        private void OnEventRaised(FlowEvent flowEvent)
        {
            if (flowEvent == null)
                return;

            IList<Tuple<string,ICommand>> commands = GetCommands(flowEvent);
            foreach (var command in commands)
            {
                if (!(flowEvent is CommandProcessingEvent))
                    eventStore.AddEvent(new OnStartProcessingCommand(flowEvent.ContextOfEvent, command.Item2));

                var events = command.Item2.Execute();

                Action onComplete = () =>
                {
                    if (!(flowEvent is CommandProcessingEvent))
                        eventStore.AddEvent(new OnEndProcessingCommand(flowEvent.ContextOfEvent, command.Item2));
                };

                string stepName = command.Item1;// This is needed to capture variable also, we should use command variable in this lambda

                events.Subscribe(currentEvent =>
                {
                    if (currentEvent == null)
                        return;

                    if (currentEvent.ContextOfEvent == null)
                        currentEvent.ContextOfEvent = flowEvent.ContextOfEvent;

                    if (currentEvent.StepName == null && stepName != null)
                        currentEvent.StepName = stepName;

                    eventStore.AddEvent(currentEvent);
                }, ex => Console.WriteLine(ex), onComplete);
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
                return commandFactory.Get(flowEvent.ContextOfEvent, eventToCommandMapping[eventType], flowEvent.StepName);
            }
            else
            {
                return new List<Tuple<string, ICommand>>();
            }
        }


        private void AddEventMapping(LinkEventToCommandAttribute link)
        {
            if (eventToCommandMapping.ContainsKey(link.FlowEvent))
                eventToCommandMapping[link.FlowEvent].Add(link.Command);
            else
                eventToCommandMapping.Add(link.FlowEvent, new List<Type>() { link.Command });
        }
    }
}
