﻿using Akkatecture.Aggregates;
using Akkatecture.Core;

namespace Akkatecture.Sagas
{
    public interface ISagaHandles<TAggregate, in TIdentity, in TAggregateEvent> : ISaga
        where TAggregateEvent : IAggregateEvent<TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        Task Handle(IDomainEvent<TAggregate, TIdentity, TAggregateEvent> domainEvent);
    }
}