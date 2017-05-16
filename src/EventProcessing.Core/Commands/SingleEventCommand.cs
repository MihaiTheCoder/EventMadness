using EventProcessing.Core.EventStore;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;

namespace EventProcessing.Core.Commands
{
    public abstract class SingleEventCommand : ICommand
    {

        public abstract FlowEvent SingleReturnExecute();


        public IObservable<FlowEvent> Execute()
        {            
            return Observable.Return(SingleReturnExecute());
        }
    }
}
