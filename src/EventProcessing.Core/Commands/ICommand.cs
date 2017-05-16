using EventProcessing.Core.EventStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventProcessing.Core
{
    public interface ICommand
    {
        IObservable<FlowEvent> Execute();
    }
}
