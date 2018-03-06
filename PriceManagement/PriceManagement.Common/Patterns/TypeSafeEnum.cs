using System.Collections.Generic;

namespace PriceManagement.Common.Patterns
{
    public abstract class TypeSafeEnum<T>
        where T : class
    {
        private static readonly Dictionary<string, object> _ordinals = new Dictionary<string, object>();

        public static T GetByName(string name)
        {
            return (T) (_ordinals.TryGetValue(name, out object value) ? value : null);
        }

        public string Name { get; }

        protected TypeSafeEnum(string name)
        {
            _ordinals.Add(name, this);
            Name = name;
        }
    }
}
