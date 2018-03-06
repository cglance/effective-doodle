using System.Collections.Generic;
using System.Threading.Tasks;
using PriceManagement.Common;
using PriceManagement.Common.DataTypes;

namespace PriceManagement.Domain.Contracting
{
    public interface IContractVendorRepository
    {
        Task<IList<ContractVendor>> GetVendorsInRole(int contractId, ContractVendorRole role, Date asOf);
    }
}
