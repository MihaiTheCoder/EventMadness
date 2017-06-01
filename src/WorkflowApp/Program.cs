using EventProcessing.Core.CommandCreation;
using EventProcessing.Core.DocumentationGenerator;
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
            AttributeFlowReader flowReader = new AttributeFlowReader(typeof(GuessTheNumberFlow));
            var flow = flowReader.GetFlatFlow();
            var activityDiagramBuilder = new ActivityDiagramBuilder();
            var activityDiagram = activityDiagramBuilder.GetActivityDiagram(flow, true);

            SimpleFlow simpleFlow = new SimpleFlow(new InMemoryEventStore());
            simpleFlow.Start();
            

            GuessTheNumberFlow guessTheNumber = new GuessTheNumberFlow(new InMemoryEventStore());
            guessTheNumber.Start();


        }
    }
}
