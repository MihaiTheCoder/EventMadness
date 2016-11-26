using System;
using System.Collections.Generic;

namespace EventProcessing.Core.EventStore
{
    public abstract class FlowContext : IDisposable
    {
        public FlowContext()
        {
            ChildContexts = new List<FlowContext>();
        }
        public List<FlowContext> ChildContexts { get; set; }

        public FlowContext ParentContext { get; set; }

        public abstract object Identifier { get; }        

        private event EventHandler DisposedEvent;
        public event EventHandler Disposed
        {
            add { DisposedEvent += value; }
            remove { DisposedEvent -= value; }
        }

        bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            lock (Identifier)
            {
                if (disposing)
                {
                    DisposedEvent?.Invoke(this, null);

                    foreach (var childContext in ChildContexts)
                    {
                        childContext.Dispose();
                    }
                }
                disposed = true;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is FlowContext)
            {
                Equals(Identifier, ((FlowContext)obj).Identifier);
            }
            else
            {
                Equals(Identifier, obj);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }

        public override string ToString()
        {
            return Identifier.ToString();
        }
    }
}