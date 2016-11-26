using EventProcessing.Core.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Reactive.Linq;

namespace EventProcessing.Core.EventStore
{
    public class InMemoryEventStore : IEventStore
    {
        ConcurrentDictionary<FlowContext, ReplaySubject<FlowEvent>> flowEvents;

        public InMemoryEventStore()
        {
            flowEvents = new ConcurrentDictionary<FlowContext, ReplaySubject<FlowEvent>>();
        }

        public void AddEvent(FlowEvent flowEvent)
        {
            flowEvents.AddOrUpdate(flowEvent.ContextOfEvent,
                (context) => GetObservableWithElement(flowEvent),//if context does not exist create new observable
                (context, obs) =>//If context exists, add event to context and return it
                {
                    obs.OnNext(flowEvent);
                    return obs;
                });
        }

        public IList<FlowEvent> GetCurrentEvents(FlowContext context, Type flowEventType)
        {
            var events = Subscribe(context, flowEventType);
            
            List<FlowEvent> eventList = new List<FlowEvent>();
            events.Subscribe(e => eventList.Add(e));
            
            return eventList;           
        }

        public FlowEvent GetLatestEvent(FlowContext context, Type flowEventType)
        {
            var events = Subscribe(context, flowEventType);

            var latestEvent = events.Latest().FirstOrDefault();

            if (latestEvent != null)
                return latestEvent;

            var currentContext = context.ParentContext;
            while (currentContext != null)
            {
                latestEvent = Subscribe(currentContext, flowEventType).Latest().FirstOrDefault();

                if (latestEvent == null)
                    currentContext = currentContext.ParentContext;
                else
                    return latestEvent;

            }
            string errorMessage = string.Format("Could not find event of type:{0} in context:{1} or any of the parrent contexts", flowEventType.FullName, context);
            throw new ArgumentOutOfRangeException(errorMessage);

        }

        public IObservable<FlowEvent> Subscribe(FlowContext context, Type flowEventType)
        {
            ReplaySubject<FlowEvent> events;
            if (flowEvents.TryGetValue(context, out events))
            {
                return events.Where(e => e.ContextOfEvent == context && e.IsSameOrChildOf(flowEventType));
            }
            else
            {
                throw new ArgumentOutOfRangeException("Could not find context:" + context);
            }
        }

        private ReplaySubject<FlowEvent> GetObservableWithElement(FlowEvent flowEvent)
        {
            var observable = new ReplaySubject<FlowEvent>();
            observable.OnNext(flowEvent);
            flowEvent.ContextOfEvent.Disposed += OnContextDisposed;
            return observable;
        }

        private void OnContextDisposed(object sender, EventArgs e)
        {
            FlowContext context = (FlowContext)sender;
            ReplaySubject<FlowEvent> replaySubject;
            flowEvents.TryRemove(context, out replaySubject);

            if (replaySubject != null)
            {
                replaySubject.OnCompleted();
                replaySubject.Dispose();
            }
        }
    }
}
