using EventProcessing.Core;
using EventProcessing.Core.Attributes;
using EventProcessing.Core.EventStore;
using EventProcessing.Core.EventStore.ConcreteContexts;
using EventProcessing.Core.FlowExecutors;
using EventProcessing.Core.StandardEvents;
using System;

namespace WorkflowApp.SimpleWorkflow
{
    [LinkEventToCommand(typeof(OnStart), typeof(GenerateMessage))]
    [LinkEventToCommand(typeof(OnMessageGenerated), typeof(PrintGeneratedMessage))]
    public class SimpleFlow : AttributeFlowTemplate
    {
        public SimpleFlow(IEventStore eventStore) : base(eventStore)
        {
        }

        private FlowEvent _initialEvent = new OnStart(new StringContext("1"));
        public override FlowEvent InitialEvent { get { return _initialEvent; } }

        protected override void RegisterCommands()
        {
            commandFactory.RegisterCommand((context) => new GenerateMessage(eventRaiserFactory.Get(context)));
            commandFactory.RegisterCommand((context) => 
            new PrintGeneratedMessage(eventRaiserFactory.Get(context), Get<OnMessageGenerated>(context).GeneratedMessage));
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
    public class GenerateMessage : ICommand
    {
        IEventRaiser eventRaiser;
        public GenerateMessage(IEventRaiser eventRaiser)
        {
            this.eventRaiser = eventRaiser;
        }

        public void Execute()
        {
            string generatedMessage = "First message";
            eventRaiser.RaiseEventInCurrentContext(new OnMessageGenerated(generatedMessage));
        }
    }

    [MayRaise(typeof(OnMessageGenerated))]
    public class PrintGeneratedMessage : ICommand
    {
        IEventRaiser eventRaiser;
        string generatedMessage;
        public PrintGeneratedMessage(IEventRaiser eventRaiser, string generatedMessage)
        {
            this.eventRaiser = eventRaiser;
            this.generatedMessage = generatedMessage;
        }

        public void Execute()
        {
            Console.WriteLine(generatedMessage);
            eventRaiser.RaiseEventInCurrentContext(new OnMessagePrinted());
        }
    }
}
