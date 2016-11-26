using System;
using System.Collections.Concurrent;

namespace EventProcessing.Core.EventStore
{
    public interface IEventRaiserFactory
    {
        IEventRaiser Get(FlowContext context);
    }
    public class EventRaiserFactory : IEventRaiserFactory
    {
        IEventStore eventStore;
        ConcurrentDictionary<FlowContext, EventRaiser> cachedEventRaisers;
        
        public EventRaiserFactory(IEventStore eventStore)
        {
            this.eventStore = eventStore;
            cachedEventRaisers = new ConcurrentDictionary<FlowContext, EventRaiser>();
        }

        public IEventRaiser Get(FlowContext context)
        {
            return cachedEventRaisers.GetOrAdd(context, (c) =>
            {
                c.Disposed += contextDisposed;
                return new EventRaiser(eventStore, c);
            });
        }

        private void contextDisposed(object sender, EventArgs e)
        {
            FlowContext context = (FlowContext)sender;
            EventRaiser er;
            cachedEventRaisers.TryRemove(context, out er);
        }
    }
}
