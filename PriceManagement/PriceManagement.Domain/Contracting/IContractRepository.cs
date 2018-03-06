using System;
using System.Collections.Generic;
using System.Text;
using PriceManagement.Common;
using PriceManagement.Common.DataTypes;

namespace PriceManagement.Domain.Contracting
{
    public interface IContractRepository
    {
        Contract GetContractByNumber(int contractNumber, Date asOf);
    }
}
