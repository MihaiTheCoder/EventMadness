using EventProcessing.Core.EventStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkflowApp.GuessTheNumber.Events
{
    public class OnValueNotAnIntegerEntered : FlowEvent
    {
        private string valueEntered;

        public OnValueNotAnIntegerEntered(string valueEntered)
        {
            this.valueEntered = valueEntered;
        }
    }
}
