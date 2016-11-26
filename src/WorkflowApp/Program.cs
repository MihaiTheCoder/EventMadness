using EventProcessing.Core.CommandCreation;
using EventProcessing.Core.EventStore;
using EventProcessing.Core.EventStore.ConcreteContexts;
using EventProcessing.Core.FlowExecutors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkflowApp.SimpleWorkflow;

namespace WorkflowApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var eventStore = new InMemoryEventStore();
            var commandFactory = new CommandFactory(eventStore);
            AnnotationsFlowExecutor annFlowExecutor = new AnnotationsFlowExecutor(typeof(SimpleFlow),
                eventStore,
                commandFactory);

            SimpleFlow simpleFlow = new SimpleFlow(commandFactory, eventStore);
            simpleFlow.RegisterCommands();

            annFlowExecutor.Execute(new OnStart { ContextOfEvent = new StringIdentifiedContext("1") });

            
        }
    }
}
