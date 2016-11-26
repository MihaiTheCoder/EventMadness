using EventProcessing.Core.EventStore;

namespace WorkflowApp.GuessTheNumber.Events
{
    public class OnNumberGuessed : FlowEvent
    {
        private int randomValue;

        public OnNumberGuessed(int randomValue)
        {
            this.randomValue = randomValue;
        }
    }
}
