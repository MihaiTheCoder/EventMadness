using EventProcessing.Core;
using EventProcessing.Core.Commands;
using EventProcessing.Core.EventStore;
using System;
using WorkflowApp.GuessTheNumber.Events;

namespace WorkflowApp.GuessTheNumber.Commands
{
    public class PrintSuccess : SingleEventCommand<SuccessPrinted>
    {
        int numberOfAttempts;
        public PrintSuccess(int numberOfAttempts)
        {
            this.numberOfAttempts = numberOfAttempts;
        }
        public override SuccessPrinted SingleReturnExecute()
        {
            Console.WriteLine("Congrats, you guessed the number correctly, using {0} guesses", numberOfAttempts);
            return new SuccessPrinted();
        }
    }
}
