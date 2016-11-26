using EventProcessing.Core;
using EventProcessing.Core.Attributes;
using EventProcessing.Core.EventStore;
using System;
using WorkflowApp.GuessTheNumber.Events;

namespace WorkflowApp.GuessTheNumber.Commands
{

    [MayRaise(typeof(OnMistakePrinted), ExtraMessage = ErrorMessage)]
    internal class PrintNotANumberCommand : InvalidValueCommand
    {
        public const string ErrorMessage = "{0} is not a number";
        public PrintNotANumberCommand(IEventRaiser eventRaiser, string valueEntered) : 
            base(eventRaiser, string.Format(ErrorMessage, valueEntered))
        {
        }
    }

    [MayRaise(typeof(OnMistakePrinted), ExtraMessage = ErrorMessage)]
    internal class PrintValueToHighCommand : InvalidValueCommand
    {
        public const string ErrorMessage = "Value to high. Please try again";
        public PrintValueToHighCommand(IEventRaiser eventRaiser) : base(eventRaiser, ErrorMessage)
        {
        }
    }

    [MayRaise(typeof(OnMistakePrinted), ExtraMessage=ErrorMessage)]
    internal class PrintValueToLowCommand : InvalidValueCommand
    {
        public const string ErrorMessage = "Value to low. Please try again.";
        public PrintValueToLowCommand(IEventRaiser eventRaiser) : base(eventRaiser, "Value to low. Please try again.")
        {
        }
    }

    public abstract class InvalidValueCommand : ICommand
    {
        IEventRaiser eventRaiser;
        string messageToPrint;

        public InvalidValueCommand(IEventRaiser eventRaiser, string messageToPrint)
        {
            this.eventRaiser = eventRaiser;
            this.messageToPrint = messageToPrint;
        }

        public void Execute()
        {
            Console.WriteLine(messageToPrint);
            eventRaiser.RaiseEventInCurrentContext(new OnMistakePrinted());
        }
    }
}
