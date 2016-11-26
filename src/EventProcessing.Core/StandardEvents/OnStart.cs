using EventProcessing.Core.EventStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventProcessing.Core.StandardEvents
{
    public class OnStart : FlowEvent
    {
        public OnStart(FlowContext context)
        {
            ContextOfEvent = context;
        }
    }
}
