using System;

namespace EventProcessing.Core.Attributes
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class MayRaiseAttribute : Attribute
    {
        public MayRaiseAttribute(Type eventThatMayBeRaised)
        {
            EventThatMayBeRaised = eventThatMayBeRaised;
        }

        public Type EventThatMayBeRaised { get; private set; }

        // This is a named argument
        public string Condition { get; set; }
        public string ExtraMessage { get; set; }
    }
}
