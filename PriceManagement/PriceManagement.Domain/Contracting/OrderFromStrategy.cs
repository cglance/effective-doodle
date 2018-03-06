using PriceManagement.Common.Patterns;

namespace PriceManagement.Domain
{
    public class OrderFromStrategy : TypeSafeEnum<OrderFromStrategy>
    {
        public static readonly OrderFromStrategy Direct = new OrderFromStrategy("Direct", true, false);

        public static readonly OrderFromStrategy Distributed = new OrderFromStrategy("Distributed", false, true);

        public static readonly OrderFromStrategy Both = new OrderFromStrategy("Both", true, true);

        private OrderFromStrategy(string name, bool isDirectOrderingAllowed, bool isDistributedOrderingAllowed)
            : base(name)
        {
            IsDirectOrderingAllowed = isDirectOrderingAllowed;
            IsDistributedOrderingAllowed = isDistributedOrderingAllowed;
        }

        public bool IsDirectOrderingAllowed { get; }

        public bool IsDistributedOrderingAllowed { get; }
    }
}
