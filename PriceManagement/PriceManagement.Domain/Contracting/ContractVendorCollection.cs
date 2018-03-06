using System.Linq;
using System.Threading.Tasks;
using PriceManagement.Common;
using PriceManagement.Common.DataTypes;

namespace PriceManagement.Domain.Contracting
{
    public class ContractVendorCollection
    {
        private readonly IContractVendorRepository _repo;

        private readonly Contract _contract;

        public ContractVendorCollection(IContractVendorRepository repo, Contract contract)
        {
            _repo = repo;
            _contract = contract;
        }

        public async Task<ContractVendor> GetContractedVendor(Date asOf = null)
        {
            return (await _repo.GetVendorsInRole(_contract.Id, ContractVendorRole.ContractVendor, asOf.OrToday())).Single();
        }
    }
}
