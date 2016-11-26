using EventProcessing.Core.Attributes;
using EventProcessing.Core.CommandCreation;
using EventProcessing.Core.EventStore;
using EventProcessing.Core.StandardEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EventProcessing.Core.FlowExecutors
{
    public class AttributeBasedFlowExecutor : IFlowExecutor
    {
        IEventStore eventStore;
        Type flowClass;
        IDictionary<Type, List<Type>> eventToCommandMapping;
        ICommandFactory commandFactory;
        public AttributeBasedFlowExecutor(Type flowClass, IEventStore eventStore, ICommandFactory commandFactory)
        {
            this.eventStore = eventStore;
            this.flowClass = flowClass;
            eventToCommandMapping = new Dictionary<Type, List<Type>>();
            this.commandFactory = commandFactory;
            eventStore.SubscribeToAllEvents().Subscribe(OnEventRaised);
        }

        private void OnEventRaised(FlowEvent flowEvent)
        {
            IList<ICommand> commands = GetCommands(flowEvent);
            foreach (var command in commands)
            {
                if (!(flowEvent is CommandProcessingEvent))
                    eventStore.AddEvent(new OnStartProcessingCommand(flowEvent.ContextOfEvent, command));

                command.Execute();

                if (!(flowEvent is CommandProcessingEvent))
                    eventStore.AddEvent(new OnEndProcessingCommand(flowEvent.ContextOfEvent, command));
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

        private IList<ICommand> GetCommands(FlowEvent flowEvent)
        {
            var eventType = flowEvent.GetType();
            if (eventToCommandMapping.ContainsKey(eventType))
            {
                return commandFactory.Get(flowEvent.ContextOfEvent, eventToCommandMapping[eventType]);
            }
            else
            {
                return new List<ICommand>();
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
