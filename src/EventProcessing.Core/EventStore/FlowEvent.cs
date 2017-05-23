namespace EventProcessing.Core.EventStore
{
    public class FlowEvent
    {
        public FlowContext ContextOfEvent { get; set; }

        private string commandName;

        public string CommandName
        {
            get
            {
                if (commandName == null)
                    commandName = "";
                return commandName;
            }
            set { commandName = value; }
        }

    }
}