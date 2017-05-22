using System.ComponentModel;
using EventProcessing.Core.EventStore;

namespace WorkflowApp.GuessTheNumber.Events
{
    [Description("Raised when the generated number is guessed")]
    public class OnNumberGuessed : FlowEvent
    {
        private int randomValue;

        public OnNumberGuessed(int randomValue)
        {
            this.randomValue = randomValue;
        }
    }
}
