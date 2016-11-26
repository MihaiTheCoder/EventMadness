using EventProcessing.Core.EventStore;

namespace WorkflowApp.GuessTheNumber.Events
{
    public class OnRandomNumberGenerated : FlowEvent
    {
        public OnRandomNumberGenerated(int randomNumber)
        {
            Value = randomNumber;
        }

        public int Value { get; internal set; }
    }
}
