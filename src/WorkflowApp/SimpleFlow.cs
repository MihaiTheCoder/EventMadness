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
    [LinkEventToCommand(typeof(OnStart), typeof(GenerateMessage), actualStep: "Le start context")]
    [LinkEventToCommand(typeof(OnMessageGenerated), typeof(PrintGeneratedMessage), sourceStep:"El ciupacapra")]
    public class SimpleFlow : AttributeFlowTemplate
    {
        public SimpleFlow(IEventStore eventStore) : base(eventStore)
        {
        }

        private FlowEvent _initialEvent = new OnStart(new StringContext("1"));
        public override FlowEvent InitialEvent { get { return _initialEvent; } }

        protected override void RegisterCommands()
        {
            commandFactory.RegisterCommand(context => new GenerateMessage());
            commandFactory.RegisterCommand(context => new PrintGeneratedMessage(Get<OnMessageGenerated>(context).GeneratedMessage));
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
    public class GenerateMessage : SingleEventCommand
    {
        public override FlowEvent SingleReturnExecute()
        {
            return new OnMessageGenerated("First message");
        }
    }

    [MayRaise(typeof(OnMessageGenerated))]
    public class PrintGeneratedMessage : SingleEventCommand
    {
        string generatedMessage;
        public PrintGeneratedMessage(string generatedMessage)
        {
            this.generatedMessage = generatedMessage;
        }

        public override FlowEvent SingleReturnExecute()
        {
            Console.WriteLine(generatedMessage);
            return new OnMessagePrinted();
        }
    }
}
