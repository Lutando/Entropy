﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Projections.Prototype
{
    public class EventMap<TContext> : IEventMap<TContext>
    {
        private readonly Dictionary<Type, List<Handler>> mappings = new Dictionary<Type, List<Handler>>();
        private readonly List<Func<object, TContext, Task<bool>>> filters =
            new List<Func<object, TContext, Task<bool>>>();

        internal void Add<TEvent>(Func<TEvent, TContext, Task> action)
        {
            if (!mappings.ContainsKey(typeof(TEvent)))
            {
                mappings[typeof(TEvent)] = new List<Handler>();
            }

            mappings[typeof(TEvent)].Add((@event, context) => action((TEvent)@event, context));
        }

        internal void AddFilter(Func<object, TContext, Task<bool>> filter)
        {
            filters.Add(filter);
        }

        public async Task<bool> Handle(object anEvent, TContext context)
        {
            if (anEvent == null)
            {
                throw new ArgumentNullException(nameof(anEvent));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (await PassesFilter(anEvent, context))
            {
                Type eventType = anEvent.GetType();

                List<Handler> handlers = GetHandlersForType(eventType);

                if (handlers.Any())
                {
                    foreach (Handler handler in handlers)
                    {
                        await handler(anEvent, context);
                    }

                    return true;
                }
            }

            return false;
        }

        private async Task<bool> PassesFilter(object anEvent, TContext context)
        {
            if (filters.Count > 0)
            {
                bool[] results = await Task.WhenAll(filters.Select(filter => filter(anEvent, context)));

                return results.All(x => x);
            }
            else
            {
                return true;
            }
        }

        private List<Handler> GetHandlersForType(Type eventType)
        {
            var handlers = new List<Handler>();
            Type baseType = mappings.Keys.FirstOrDefault(key => eventType.GetTypeInfo().IsSubclassOf(key));
            if (baseType != null)
            {
                handlers.AddRange(mappings[baseType]);
            }

            if (mappings.TryGetValue(eventType, out var concreteTypeHandlers))
            {
                handlers.AddRange(concreteTypeHandlers);
            }

            return handlers;
        }

        private delegate Task Handler(object @event, TContext context);
    }
}
