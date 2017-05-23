using EventProcessing.Core;
using EventProcessing.Core.Attributes;
using EventProcessing.Core.Commands;
using EventProcessing.Core.EventStore;
using System;
using WorkflowApp.GuessTheNumber.Events;

namespace WorkflowApp.GuessTheNumber.Commands
{
    [MayRaise(typeof(OnNumberEntered))]
    public class EnterNumber : SingleEventCommand
    {
        public override FlowEvent SingleReturnExecute()
        {
            Console.Write("Guess the number:");
            string valueEntered = Console.ReadLine();
            return new OnNumberEntered(valueEntered);
        }
    }
}
