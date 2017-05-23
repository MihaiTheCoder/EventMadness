using System;
using EventProcessing.Core.Commands;
using EventProcessing.Core.EventStore;

namespace WorkflowApp
{
    public class PrintNumberRange : SingleEventCommand<OnNumberRangePrinted>
    {
        public override OnNumberRangePrinted SingleReturnExecute()
        {
            Console.WriteLine("Enter a number between 1 and 100");
            return new OnNumberRangePrinted();
        }
    }
}