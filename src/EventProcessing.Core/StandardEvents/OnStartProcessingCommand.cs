using EventProcessing.Core.EventStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventProcessing.Core.StandardEvents
{
    public class OnStartProcessingCommand : CommandProcessingEvent
    {
        public OnStartProcessingCommand(FlowContext context, ICommand command) : base(context, command)
        {
            
        }
    }
}
