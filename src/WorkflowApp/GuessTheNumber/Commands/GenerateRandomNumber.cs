using EventProcessing.Core;
using EventProcessing.Core.Attributes;
using EventProcessing.Core.EventStore;
using System;
using WorkflowApp.GuessTheNumber.Events;

namespace WorkflowApp.GuessTheNumber.Commands
{
    [MayRaise(typeof(OnRandomNumberGenerated))]
    public class GenerateRandomNumber : ICommand
    {
        private IEventRaiser eventRaiser;

        public GenerateRandomNumber(IEventRaiser eventRaiser)
        {
            this.eventRaiser = eventRaiser;
        }

        public void Execute()
        {
            Random r = new Random(1);
            int randomNumber = r.Next(100);

            eventRaiser.RaiseEventInCurrentContext(new OnRandomNumberGenerated(randomNumber));
        }
    }
}
