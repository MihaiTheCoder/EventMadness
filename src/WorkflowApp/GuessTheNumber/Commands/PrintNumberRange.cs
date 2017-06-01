using System;
using EventProcessing.Core.Commands;
using EventProcessing.Core.Attributes;

namespace WorkflowApp
{
    [MayRaise(typeof(OnNumberRangePrinted))]
    public class PrintNumberRange : SingleEventCommand<OnNumberRangePrinted>
    {
        public override OnNumberRangePrinted SingleReturnExecute()
        {
            Console.WriteLine("Enter a number between 1 and 100");
            return new OnNumberRangePrinted();
        }
    }
}