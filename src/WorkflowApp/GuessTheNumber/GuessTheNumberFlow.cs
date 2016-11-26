using EventProcessing.Core.FlowExecutors;
using System;
using EventProcessing.Core.EventStore;
using EventProcessing.Core.Attributes;
using EventProcessing.Core.StandardEvents;
using EventProcessing.Core.EventStore.ConcreteContexts;
using EventProcessing.Core;
using WorkflowApp.GuessTheNumber.Commands;
using WorkflowApp.GuessTheNumber.Events;

namespace WorkflowApp
{    
    [LinkEventToCommand(typeof(OnStart), typeof(GenerateRandomNumber))]
    [LinkEventToCommand(typeof(OnRandomNumberGenerated), typeof(EnterNumberCommand))]
    [LinkEventToCommand(typeof(OnNumberEntered), typeof(ValidateNumber))]

    //Validate number events
    [LinkEventToCommand(typeof(OnNumberGuessed), typeof(PrintSuccess))]
    [LinkEventToCommand(typeof(OnValueToLow), typeof(PrintValueToLowCommand))]
    [LinkEventToCommand(typeof(OnValueToHigh), typeof(PrintValueToHighCommand))]
    [LinkEventToCommand(typeof(OnValueNotAnIntegerEntered), typeof(PrintNotANumberCommand))]

    [LinkEventToCommand(typeof(OnMistakePrinted), typeof(EnterNumberCommand))]
    public class GuessTheNumberFlow : AttributeFlowTemplate
    {
        public GuessTheNumberFlow(IEventStore eventStore) : base(eventStore)
        {
        }

        private FlowEvent _initialEvent = new OnStart(new StringContext("1"));
        public override FlowEvent InitialEvent { get { return _initialEvent; } }

        protected override void RegisterCommands()
        {
            commandFactory.RegisterCommand((context) => new GenerateRandomNumber(eventRaiserFactory.Get(context)));
            commandFactory.RegisterCommand((context) => new EnterNumberCommand(eventRaiserFactory.Get(context)));
            commandFactory.RegisterCommand((context) => new ValidateNumber(eventRaiserFactory.Get(context),
                Get<OnNumberEntered>(context).Value, Get<OnRandomNumberGenerated>(context).Value));
            commandFactory.RegisterCommand(context => new PrintSuccess(GetCount<OnNumberEntered>(context)));
            commandFactory.RegisterCommand(context => new PrintValueToHighCommand(eventRaiserFactory.Get(context)));
            commandFactory.RegisterCommand(context => new PrintValueToLowCommand(eventRaiserFactory.Get(context)));
            commandFactory.RegisterCommand(context =>
            new PrintNotANumberCommand(eventRaiserFactory.Get(context), Get<OnNumberEntered>(context).Value));
        }
    }
}
