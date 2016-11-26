using EventProcessing.Core;
using EventProcessing.Core.Attributes;
using EventProcessing.Core.CommandCreation;
using EventProcessing.Core.EventStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkflowApp.SimpleWorkflow
{
    [LinkEventToCommand(typeof(OnStart), typeof(GenerateMessage))]
    [LinkEventToCommand(typeof(OnMessageGenerated), typeof(PrintGeneratedMessage))]
    public class SimpleFlow
    {
        ICommandFactory commandFactory;
        IEventRaiserFactory eventRaiserFactory;
        IEventStore eventStore;

        public SimpleFlow(ICommandFactory commandFactory, IEventStore eventStore)
        {
            this.commandFactory = commandFactory;
            this.eventRaiserFactory = eventStore.GetEventRaiserFactory();
            this.eventStore = eventStore;
        }

        public void RegisterCommands()
        {
            commandFactory.RegisterCommand((context) => new GenerateMessage(eventRaiserFactory.Get(context)));
            commandFactory.RegisterCommand((context) => 
            new PrintGeneratedMessage(eventRaiserFactory.Get(context), Get<OnMessageGenerated>(context).GeneratedMessage));
        }

        private TEvent Get<TEvent>(FlowContext flowContext) where TEvent : FlowEvent
        {
            return eventStore.GetLatestEvent<TEvent>(flowContext);
        }
    }

    public class OnStart : FlowEvent { }

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
