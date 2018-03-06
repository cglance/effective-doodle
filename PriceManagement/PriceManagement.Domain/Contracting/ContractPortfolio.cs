using PriceManagement.Common;
using PriceManagement.Common.DataTypes;

namespace PriceManagement.Domain.Contracting
{
    public class ContractPortfolio
    {
        private IContractRepository _repo;

        public Contract GetContractByNumber(int contractNumber, Date asOf = null)
        {
            return _repo.GetContractByNumber(contractNumber, asOf.OrToday());
        }
    }
}
