using EventProcessing.Core.Helpers;
using System;
using System.Collections.Generic;

namespace EventProcessing.Core.EventStore
{
    public interface IEventStore
    {
        void AddEvent(FlowEvent flowEvent);

        FlowEvent GetLatestEvent(FlowContext context, Type flowEventType);

        TEvent GetLatestEvent<TEvent>(FlowContext context) where TEvent : FlowEvent;

        IObservable<FlowEvent> Subscribe(FlowContext context, Type flowEventType);

        IObservable<FlowEvent> SubscribeToAllEvents();

        IList<FlowEvent> GetCurrentEvents(FlowContext context, Type flowEventType);

        IEventRaiserFactory GetEventRaiserFactory();
    }
}
