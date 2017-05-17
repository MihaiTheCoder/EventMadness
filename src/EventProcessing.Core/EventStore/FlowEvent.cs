namespace EventProcessing.Core.EventStore
{
    public class FlowEvent
    {
        public FlowContext ContextOfEvent { get; set; }

        private string stepName;

        public string StepName
        {
            get
            {
                if (stepName == null)
                    stepName = "";
                return stepName;
            }
            set { stepName = value; }
        }

    }
}