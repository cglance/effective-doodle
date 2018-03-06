using PriceManagement.Common.Patterns;

namespace PriceManagement.Domain
{
    public class MarkupStrategy : TypeSafeEnum<MarkupStrategy>
    {
        public static readonly MarkupStrategy Embedded = new MarkupStrategy("Embedded");
        public static readonly MarkupStrategy Computed = new MarkupStrategy("Computed");
        public static readonly MarkupStrategy None = new MarkupStrategy("None");

        private MarkupStrategy(string name) : base(name)
        {
        }
    }
}
