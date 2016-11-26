using EventProcessing.Core;
using EventProcessing.Core.Attributes;
using EventProcessing.Core.EventStore;
using WorkflowApp.GuessTheNumber.Events;

namespace WorkflowApp.GuessTheNumber.Commands
{
    [MayRaise(typeof(OnNumberGuessed), Condition = "The random number was guessed")]
    [MayRaise(typeof(OnValueToLow), Condition = "Value is lower than the random generated number")]
    [MayRaise(typeof(OnValueToHigh), Condition = "Value is higher than the random generated number")]
    [MayRaise(typeof(OnValueNotAnIntegerEntered), Condition = "The value is not an integer")]
    public class ValidateNumber : ICommand
    {
        private IEventRaiser eventRaiser;
        private string valueEntered;
        private int randomValue;

        public ValidateNumber(IEventRaiser eventRaiser, string valueEntered, int randomValue)
        {
            this.eventRaiser = eventRaiser;
            this.valueEntered = valueEntered;
            this.randomValue = randomValue;
        }

        public void Execute()
        {
            int valueAsInt;
            if (int.TryParse(valueEntered, out valueAsInt))
            {
                if (valueAsInt == randomValue)
                    eventRaiser.RaiseEventInCurrentContext(new OnNumberGuessed(randomValue));
                else if (valueAsInt < randomValue)
                    eventRaiser.RaiseEventInCurrentContext(new OnValueToLow(valueAsInt));
                else
                    eventRaiser.RaiseEventInCurrentContext(new OnValueToHigh(valueAsInt));
            }
            else
                eventRaiser.RaiseEventInCurrentContext(new OnValueNotAnIntegerEntered(valueEntered));
        }
    }
}
