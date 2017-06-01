using EventProcessing.Core.EventStore;
using System;
using System.Reactive.Linq;

namespace EventProcessing.Core.Commands
{
    public abstract class SingleEventCommand<TEvent> : ICommand where TEvent : FlowEvent
    {

        public abstract TEvent SingleReturnExecute();


        public IObservable<FlowEvent> Execute()
        {            
            var observable = Observable.Return(SingleReturnExecute());
            
            return observable;
        }
    }

    public abstract class SingleEventCommand : ICommand
    {
        public abstract FlowEvent SingleReturnExecute();


        public IObservable<FlowEvent> Execute()
        {
            var observable = Observable.Return(SingleReturnExecute());

            return observable;
        }
    }
}
