using EventProcessing.Core;
using EventProcessing.Core.Attributes;
using EventProcessing.Core.Commands;
using EventProcessing.Core.EventStore;
using EventProcessing.Core.EventStore.ConcreteContexts;
using EventProcessing.Core.FlowExecutors;
using EventProcessing.Core.StandardEvents;
using System;

namespace WorkflowApp.SimpleWorkflow
{
    [LinkEventToCommand(typeof(OnStart), typeof(GenerateMessage), commandName: FIRST_GERATE_MESSAGE)]
    [LinkEventToCommand(typeof(OnMessageGenerated), typeof(PrintGeneratedMessage), sourceEventCommandName: FIRST_GERATE_MESSAGE)]
    public class SimpleFlow : AttributeFlowTemplate
    {
        public const string FIRST_GERATE_MESSAGE = "Le start context";
        public SimpleFlow(IEventStore eventStore) : base(eventStore)
        {
        }

        private FlowEvent _initialEvent = new OnStart(new StringContext("1"));
        public override FlowEvent InitialEvent { get { return _initialEvent; } }

        protected override void RegisterCommands()
        {
            commandFactory.RegisterCommand(context => new GenerateMessage(), commandName: FIRST_GERATE_MESSAGE);
            commandFactory.RegisterCommand(context => new PrintGeneratedMessage(Get<OnMessageGenerated>(context, FIRST_GERATE_MESSAGE).GeneratedMessage));
        }
    }

    public class OnMessageGenerated : FlowEvent
    {
        public OnMessageGenerated(string generatedMessage)
        {
            GeneratedMessage = generatedMessage;
        }

        public string GeneratedMessage { get; private set; }
    }

    public class OnMessagePrinted : FlowEvent { }

    [MayRaise(typeof(OnMessageGenerated))]
    public class GenerateMessage : SingleEventCommand<OnMessageGenerated>
    {
        public override OnMessageGenerated SingleReturnExecute()
        {
            return new OnMessageGenerated("First message");
        }
    }

    [MayRaise(typeof(OnMessageGenerated))]
    public class PrintGeneratedMessage : SingleEventCommand<OnMessagePrinted>
    {
        string generatedMessage;
        public PrintGeneratedMessage(string generatedMessage)
        {
            this.generatedMessage = generatedMessage;
        }

        public override OnMessagePrinted SingleReturnExecute()
        {
            Console.WriteLine(generatedMessage);
            return new OnMessagePrinted();
        }
    }
}
