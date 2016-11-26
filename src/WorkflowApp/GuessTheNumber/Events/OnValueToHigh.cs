using EventProcessing.Core.EventStore;

namespace WorkflowApp.GuessTheNumber.Events
{
    public class OnValueToHigh : FlowEvent
    {
        private int valueAsInt;

        public OnValueToHigh(int valueAsInt)
        {
            this.valueAsInt = valueAsInt;
        }
    }
}
