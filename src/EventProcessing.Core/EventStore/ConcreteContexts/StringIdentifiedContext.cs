using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventProcessing.Core.EventStore.ConcreteContexts
{
    public class StringContext : FlowContext
    {
        string identifier;
        public StringContext(string identifier)
        {
            this.identifier = identifier;
        }

        public override object Identifier
        {
            get
            {
                return identifier;
            }
        }
    }
}
