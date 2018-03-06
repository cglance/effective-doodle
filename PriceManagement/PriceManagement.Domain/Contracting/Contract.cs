using PriceManagement.Common;
using PriceManagement.Common.DataTypes;

namespace PriceManagement.Domain.Contracting
{
    public class Contract
    {
        public int Id { get; }

        public int ContractNumber { get; }

        public DateRange Effectivity { get; }

        public ContractVendorCollection Vendors { get; }
    }
}
