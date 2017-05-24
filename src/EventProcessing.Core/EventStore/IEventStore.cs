using EventProcessing.Core.Helpers;
using System;
using System.Collections.Generic;

namespace EventProcessing.Core.EventStore
{
    public interface IEventStore
    {
        void AddEvent(FlowEvent flowEvent);

        FlowEvent GetLatestEvent(FlowContext context, Type flowEventType, string commandName = "");

        TEvent GetLatestEvent<TEvent>(FlowContext flowContext, string commandName = "") where TEvent : FlowEvent;

        IObservable<TEvent> Subscribe<TEvent>(FlowContext context, string commandName = "") where TEvent : FlowEvent;

        IObservable<FlowEvent> Subscribe(FlowContext context, Type flowEventType, string commandName = "");

        IObservable<FlowEvent> SubscribeToAllEvents();

        IList<TEvent> GetCurrentEvents<TEvent>(FlowContext context) where TEvent : FlowEvent;

        IList<FlowEvent> GetCurrentEvents(FlowContext context, Type flowEventType);

        IEventRaiserFactory GetEventRaiserFactory();

    }
}
