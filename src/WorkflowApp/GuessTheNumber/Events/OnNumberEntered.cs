using EventProcessing.Core.EventStore;

namespace WorkflowApp.GuessTheNumber.Events
{
    public class OnNumberEntered : FlowEvent
    {
        public OnNumberEntered(string value)
        {
            Value = value;
        }
        public string Value { get; private set; }
    }
}
