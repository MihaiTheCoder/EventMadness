using EventProcessing.Core;
using EventProcessing.Core.Commands;
using EventProcessing.Core.EventStore;
using System;

namespace WorkflowApp.GuessTheNumber.Commands
{
    public class PrintSuccess : SingleEventCommand
    {
        int numberOfAttempts;
        public PrintSuccess(int numberOfAttempts)
        {
            this.numberOfAttempts = numberOfAttempts;
        }
        public override FlowEvent SingleReturnExecute()
        {
            Console.WriteLine("Congrats, you guessed the number correctly, using {0} guesses", numberOfAttempts);
            return null;
        }
    }
}
