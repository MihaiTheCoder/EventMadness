using EventProcessing.Core;
using EventProcessing.Core.Attributes;
using EventProcessing.Core.Commands;
using EventProcessing.Core.EventStore;
using System;
using WorkflowApp.GuessTheNumber.Events;

namespace WorkflowApp.GuessTheNumber.Commands
{

    [MayRaise(typeof(OnMistakePrinted), ExtraMessage = ErrorMessage)]
    internal class PrintNotANumber : InvalidValueCommand
    {
        public const string ErrorMessage = "{0} is not a number";
        public PrintNotANumber(string valueEntered) : 
            base(string.Format(ErrorMessage, valueEntered))
        {
        }
    }

    [MayRaise(typeof(OnMistakePrinted), ExtraMessage = ErrorMessage)]
    internal class PrintValueToHighCommand : InvalidValueCommand
    {
        public const string ErrorMessage = "Value to high. Please try again";
        public PrintValueToHighCommand() : base(ErrorMessage)
        {
        }
    }

    [MayRaise(typeof(OnMistakePrinted), ExtraMessage=ErrorMessage)]
    internal class PrintValueToLowCommand : InvalidValueCommand
    {
        public const string ErrorMessage = "Value to low. Please try again.";
        public PrintValueToLowCommand() : base("Value to low. Please try again.")
        {
        }
    }

    public abstract class InvalidValueCommand : SingleEventCommand
    {
        string messageToPrint;

        public InvalidValueCommand(string messageToPrint)
        {
            this.messageToPrint = messageToPrint;
        }

        public override FlowEvent SingleReturnExecute()
        {
            Console.WriteLine(messageToPrint);
            return new OnMistakePrinted();
        }
    }
}
