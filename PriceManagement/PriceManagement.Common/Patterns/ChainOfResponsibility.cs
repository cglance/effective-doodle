using System;
using System.Collections.Generic;

namespace PriceManagement.Common.Patterns
{
    public class ChainOfResponsibility<T>
    {
        private readonly List<Func<T, bool>> _handlers = new List<Func<T, bool>>();

        public ChainOfResponsibility()
        {
        }

        public ChainOfResponsibility(IEnumerable<Func<T, bool>> handlers)
        {
            _handlers.AddRange(handlers);
        }

        public void Add(Func<T, bool> handler)
        {
            AddHandler(handler);
        }

        public void AddHandler(Func<T, bool> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            _handlers.Add(handler);
        }

        public bool Execute(T param)
        {
            foreach (Func<T, bool> handler in _handlers)
            {
                if (handler(param))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
