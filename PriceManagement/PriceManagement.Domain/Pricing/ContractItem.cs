using PriceManagement.Common;
using PriceManagement.Common.DataTypes;
using PriceManagement.Domain.Contracting;
using PriceManagement.Domain.Item;

namespace PriceManagement.Domain.Pricing
{
    public class ContractItem
    {
        public int Id { get; }

        public ContractReference ContractVendor { get; }

        public ItemReference Item { get; }

        public DateRange Effectivity { get; }
    }
}
