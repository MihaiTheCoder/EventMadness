using EventProcessing.Core.EventStore;

namespace WorkflowApp.GuessTheNumber.Events
{
    public class OnValueToLow : FlowEvent
    {
        private int valueAsInt;

        public OnValueToLow(int valueAsInt)
        {
            this.valueAsInt = valueAsInt;
        }
    }
}
