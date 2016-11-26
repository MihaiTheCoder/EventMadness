using EventProcessing.Core.EventStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventProcessing.Core.StandardEvents
{
    public class CommandProcessingEvent : FlowEvent
    {
        protected ICommand Command;     

        public CommandProcessingEvent(FlowContext context, ICommand command)
        {
            ContextOfEvent = context;
            Command = command;
        }
    }
}
