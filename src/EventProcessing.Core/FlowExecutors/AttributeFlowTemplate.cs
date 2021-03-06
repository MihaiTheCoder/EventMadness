﻿using EventProcessing.Core.CommandCreation;
using EventProcessing.Core.EventStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventProcessing.Core.FlowExecutors
{
    public abstract class AttributeFlowTemplate
    {
        protected ICommandRegister commandFactory;
        protected IEventRaiserFactory eventRaiserFactory;
        protected IEventStore eventStore;
        protected IFlowExecutor annFlowExecutor;

        public AttributeFlowTemplate(IEventStore eventStore)
        {
            commandFactory = new CommandRegister(eventStore);
            eventRaiserFactory = eventStore.GetEventRaiserFactory();
            this.eventStore = eventStore;

            RegisterCommands();
            annFlowExecutor = GetExecutor();
        }

        public void Start()
        {
            annFlowExecutor.Execute(InitialEvent);
        }

        private IFlowExecutor GetExecutor()
        {
            return new AttributeBasedFlowExecutor(GetType(), eventStore, commandFactory);
        }

        public abstract FlowEvent InitialEvent { get; }

        protected abstract void RegisterCommands();

        protected TEvent Get<TEvent>(FlowContext flowContext) where TEvent : FlowEvent
        {
            return eventStore.GetLatestEvent<TEvent>(flowContext);
        }


        protected TEvent Get<TEvent>(FlowContext flowContext, string commandName) where TEvent : FlowEvent
        {
            return eventStore.GetLatestEvent<TEvent>(flowContext, commandName);
        }

        protected int GetCount<TEvent>(FlowContext flowContext) where TEvent : FlowEvent
        {
            return eventStore.GetCurrentEvents<TEvent>(flowContext).Count;
        }

    }
}
