using EventProcessing.Core;
using EventProcessing.Core.Attributes;
using EventProcessing.Core.EventStore;
using System;
using WorkflowApp.GuessTheNumber.Events;

namespace WorkflowApp.GuessTheNumber.Commands
{
    [MayRaise(typeof(OnNumberEntered))]
    public class EnterNumberCommand : ICommand
    {
        private IEventRaiser eventRaiser;

        public EnterNumberCommand(IEventRaiser eventRaiser)
        {
            this.eventRaiser = eventRaiser;
        }

        public void Execute()
        {
            Console.Write("Guess the number:");
            string valueEntered = Console.ReadLine();
            eventRaiser.RaiseEventInCurrentContext(new OnNumberEntered(valueEntered));
        }
    }
}
