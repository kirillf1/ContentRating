﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRating.Domain.Shared
{
    public abstract class Entity
    {
        int? _requestedHashCode;
        Guid _id;
        public Guid Id
        {
            get { return _id; }
            protected set { _id = value; }
        }

        private List<INotification> _domainEvents;
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly();

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents = _domainEvents ?? new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Entity))
            {
                return false;
            }

            if (Object.ReferenceEquals(this, obj))
            {
                return true;
            }

            if (this.GetType() != obj.GetType())
            {
                return false;
            }

            Entity item = (Entity)obj;

            return item.Id == this.Id;
        }

        public override int GetHashCode()
        {
            if (!_requestedHashCode.HasValue)
            {
                _requestedHashCode = this.Id.GetHashCode() ^ 31;
            }

            return _requestedHashCode.Value;
        }

        public static bool operator ==(Entity left, Entity right)
        {
            if (Equals(left, null))
            {
                return Equals(right, null);
            }
            else
            {
                return left.Equals(right);
            }
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }
    }
}
