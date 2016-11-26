using EventProcessing.Core.EventStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventProcessing.Core.FlowExecutors
{
    public interface IFlowExecutor
    {
        void Execute(FlowEvent initialEvent);
    }
}
