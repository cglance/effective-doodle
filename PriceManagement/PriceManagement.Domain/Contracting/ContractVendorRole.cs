using PriceManagement.Common.Patterns;

namespace PriceManagement.Domain
{
    public class ContractVendorRole : TypeSafeEnum<ContractVendorRole>
    {
        public static readonly ContractVendorRole ContractVendor = new ContractVendorRole("ContractVendor");

        public static readonly ContractVendorRole Distributor = new ContractVendorRole("Distributor");

        private ContractVendorRole(string name) : base(name)
        {
        }
    }
}
