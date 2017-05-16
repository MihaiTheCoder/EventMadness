using EventProcessing.Core;
using EventProcessing.Core.Attributes;
using EventProcessing.Core.Commands;
using EventProcessing.Core.EventStore;
using WorkflowApp.GuessTheNumber.Events;

namespace WorkflowApp.GuessTheNumber.Commands
{
    [MayRaise(typeof(OnNumberGuessed), Condition = "The random number was guessed")]
    [MayRaise(typeof(OnValueToLow), Condition = "Value is lower than the random generated number")]
    [MayRaise(typeof(OnValueToHigh), Condition = "Value is higher than the random generated number")]
    [MayRaise(typeof(OnValueNotAnIntegerEntered), Condition = "The value is not an integer")]
    public class ValidateNumber : SingleEventCommand
    {
        private string valueEntered;
        private int randomValue;

        public ValidateNumber(string valueEntered, int randomValue)
        {
            this.valueEntered = valueEntered;
            this.randomValue = randomValue;
        }

        public override FlowEvent SingleReturnExecute()
        {
            int valueAsInt;
            if (int.TryParse(valueEntered, out valueAsInt))
            {
                if (valueAsInt == randomValue)
                    return new OnNumberGuessed(randomValue);
                else if (valueAsInt < randomValue)
                    return new OnValueToLow(valueAsInt);
                else
                    return new OnValueToHigh(valueAsInt);
            }
            else
                return new OnValueNotAnIntegerEntered(valueEntered);
        }
    }
}
