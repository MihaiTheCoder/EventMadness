using EventProcessing.Core.Helpers;
using System;
using System.Collections.Generic;

namespace EventProcessing.Core.EventStore
{
    public interface IEventStore
    {
        void AddEvent(FlowEvent flowEvent);

        FlowEvent GetLatestEvent(FlowContext context, Type flowEventType);

        IObservable<FlowEvent> Subscribe(FlowContext context, Type flowEventType);

        IList<FlowEvent> GetCurrentEvents(FlowContext context, Type flowEventType);
    }
}
