using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventProcessing.Core.EventStore
{
    public interface IEventRaiser
    {
        FlowContext CurrentContext { get; }

        void RaiseEventInCurrentContext(FlowEvent flowEvent);

        void RaiseEvent(FlowEvent flowEvent);
    }

    public class EventRaiser : IEventRaiser
    {
        IEventStore eventStore;
        public EventRaiser(IEventStore eventStore, FlowContext context)
        {
            this.eventStore = eventStore;
            CurrentContext = context;
        }
        public FlowContext CurrentContext { get; private set; }

        public void RaiseEventInCurrentContext(FlowEvent flowEvent)
        {
            flowEvent.ContextOfEvent = CurrentContext;
            RaiseEvent(flowEvent);
        }

        public void RaiseEvent(FlowEvent flowEvent)
        {
            eventStore.AddEvent(flowEvent);
        }
    }
}
