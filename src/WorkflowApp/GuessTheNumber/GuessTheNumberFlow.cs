using EventProcessing.Core.FlowExecutors;
using EventProcessing.Core.EventStore;
using EventProcessing.Core.Attributes;
using EventProcessing.Core.StandardEvents;
using EventProcessing.Core.EventStore.ConcreteContexts;
using WorkflowApp.GuessTheNumber.Commands;
using WorkflowApp.GuessTheNumber.Events;

namespace WorkflowApp
{
    [LinkEventToCommand(typeof(OnStart), typeof(GenerateRandomNumber))]
    [LinkEventToCommand(typeof(OnRandomNumberGenerated), typeof(PrintNumberRange))]
    [LinkEventToCommand(typeof(OnNumberRangePrinted), typeof(EnterNumber))]
    [LinkEventToCommand(typeof(OnNumberEntered), typeof(ValidateNumber))]

    //Validate number events
    [LinkEventToCommand(typeof(OnNumberGuessed), typeof(PrintSuccess))]
    [LinkEventToCommand(typeof(OnValueToLow), typeof(PrintValueToLowCommand))]
    [LinkEventToCommand(typeof(OnValueToHigh), typeof(PrintValueToHighCommand))]
    [LinkEventToCommand(typeof(OnValueNotAnIntegerEntered), typeof(PrintNotANumber))]

    [LinkEventToCommand(typeof(OnMistakePrinted), typeof(EnterNumber))]    
    public class GuessTheNumberFlow : AttributeFlowTemplate
    {
        public GuessTheNumberFlow(IEventStore eventStore) : base(eventStore)
        {
        }

        private FlowEvent _initialEvent = new OnStart(new StringContext("1"));
        public override FlowEvent InitialEvent { get { return _initialEvent; } }

        protected override void RegisterCommands()
        {
            commandFactory.RegisterCommand(context => new GenerateRandomNumber());
            commandFactory.RegisterCommand(context => new PrintNumberRange());
            commandFactory.RegisterCommand(context => new EnterNumber());
            commandFactory.RegisterCommand(context => new ValidateNumber(Get<OnNumberEntered>(context).Value, Get<OnRandomNumberGenerated>(context).Value));
            commandFactory.RegisterCommand(context => new PrintSuccess(GetCount<OnNumberEntered>(context)));
            commandFactory.RegisterCommand(context => new PrintValueToHighCommand());
            commandFactory.RegisterCommand(context => new PrintValueToLowCommand());
            commandFactory.RegisterCommand(context => new PrintNotANumber(Get<OnNumberEntered>(context).Value));
        }
    }
}
