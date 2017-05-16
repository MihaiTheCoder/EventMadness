using EventProcessing.Core;
using EventProcessing.Core.Attributes;
using EventProcessing.Core.Commands;
using EventProcessing.Core.EventStore;
using System;
using WorkflowApp.GuessTheNumber.Events;

namespace WorkflowApp.GuessTheNumber.Commands
{
    [MayRaise(typeof(OnRandomNumberGenerated))]
    public class GenerateRandomNumber : SingleEventCommand
    {
        public GenerateRandomNumber()
        {
        }

        public override FlowEvent SingleReturnExecute()
        {
            Random r = new Random();
            int randomNumber = r.Next(100);

            return new OnRandomNumberGenerated(randomNumber);
        }
    }
}
