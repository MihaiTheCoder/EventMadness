using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventProcessing.Core.Attributes
{
    public interface EventToCommand
    {
        Type FlowEvent { get; }

        Type Command { get; }

        // This is a named argument
        string FlowIdentifier { get; }

        string SourceEventCommandName { get; }

        string CommandName { get; }
    }
}
