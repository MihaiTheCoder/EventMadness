using EventProcessing.Core;
using System;

namespace WorkflowApp.GuessTheNumber.Commands
{
    internal class PrintSuccess : ICommand
    {
        int numberOfAttempts;
        public PrintSuccess(int numberOfAttempts)
        {
            this.numberOfAttempts = numberOfAttempts;
        }
        public void Execute()
        {
            Console.WriteLine("Congrats, you guessed the number correctly, using {0} guesses", numberOfAttempts);
        }
    }
}
