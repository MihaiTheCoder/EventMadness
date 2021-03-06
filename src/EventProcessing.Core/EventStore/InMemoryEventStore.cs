﻿using EventProcessing.Core.Helpers;
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
        const string DEFAULT_COMMAND_NAME = "";
        ConcurrentDictionary<FlowContext, ReplaySubject<FlowEvent>> flowEvents;
        BehaviorSubject<FlowEvent> allFlowEventsObservable;

        public InMemoryEventStore()
        {
            flowEvents = new ConcurrentDictionary<FlowContext, ReplaySubject<FlowEvent>>();
            allFlowEventsObservable = new BehaviorSubject<FlowEvent>(null);
        }

        public void AddEvent(FlowEvent flowEvent)
        {
            if (flowEvent.ContextOfEvent == null)
                throw new ArgumentException("context must be set on each event before raising it");

            flowEvents.AddOrUpdate(flowEvent.ContextOfEvent,
                (context) => GetObservableWithElement(flowEvent),//if context does not exist create new observable
                (context, obs) =>//If context exists, add event to context and return it
                {
                    obs.OnNext(flowEvent);
                    return obs;
                });

            allFlowEventsObservable.OnNext(flowEvent);
        }

        public IList<FlowEvent> GetCurrentEvents(FlowContext context, Type flowEventType)
        {
            var events = Subscribe(context, flowEventType, DEFAULT_COMMAND_NAME);
            
            List<FlowEvent> eventList = new List<FlowEvent>();
            events.Subscribe(e => eventList.Add(e));
            
            return eventList;           
        }

        public IEventRaiserFactory GetEventRaiserFactory()
        {
            return new EventRaiserFactory(this);
        }

        public FlowEvent GetLatestEvent(FlowContext context, Type flowEventType, string commandName = DEFAULT_COMMAND_NAME)
        {
            var events = Subscribe(context, flowEventType, commandName);

            var latestEvent = events.Latest().FirstOrDefault();

            if (latestEvent != null)
                return latestEvent;

            var currentContext = context.ParentContext;
            while (currentContext != null)
            {
                latestEvent = Subscribe(currentContext, flowEventType, commandName).Latest().FirstOrDefault();

                if (latestEvent == null)
                    currentContext = currentContext.ParentContext;
                else
                    return latestEvent;

            }
            string errorMessage = string.Format("Could not find event of type:{0} in context:{1} or any of the parrent contexts", flowEventType.FullName, context);
            throw new ArgumentOutOfRangeException(errorMessage);

        }

        public TEvent GetLatestEvent<TEvent>(FlowContext context, string commandName = DEFAULT_COMMAND_NAME) where TEvent : FlowEvent
        {
            return (TEvent)GetLatestEvent(context, typeof(TEvent), commandName);
        }

        public IObservable<FlowEvent> Subscribe(FlowContext context, Type flowEventType, string commandName = DEFAULT_COMMAND_NAME)
        {
            ReplaySubject<FlowEvent> events;
            if (flowEvents.TryGetValue(context, out events))
            {
                return events.Where(e => e.ContextOfEvent == context && e.IsSameOrChildOf(flowEventType) &&( e.CommandName == commandName || e.CommandName == DEFAULT_COMMAND_NAME));
            }
            else
            {
                throw new ArgumentOutOfRangeException("Could not find context:" + context);
            }
        }

        public IObservable<FlowEvent> SubscribeToAllEvents()
        {
            return allFlowEventsObservable;
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

        public IObservable<TEvent> Subscribe<TEvent>(FlowContext context, string commandName = DEFAULT_COMMAND_NAME) where TEvent : FlowEvent
        {
            return Subscribe(context, typeof(TEvent), commandName).Select(e => (TEvent)e);
        }

        public IList<TEvent> GetCurrentEvents<TEvent>(FlowContext context) where TEvent : FlowEvent
        {
            return GetCurrentEvents(context, typeof(TEvent)).Select(e => (TEvent)e).ToList();
        }

        
    }
}
