using System;
using System.Collections.Generic;
using System.Text;
using PriceManagement.Common;
using PriceManagement.Common.DataTypes;
using PriceManagement.Domain.Vendor;

namespace PriceManagement.Domain.Contracting
{
    public class ContractVendor
    {
        public int Id { get; }

        public ContractReference Contract { get; }

        public VendorReference Vendor { get; }

        public ContractVendorRole Role { get; }

        public MarkupStrategy MarkupStrategy { get; }

        public DateRange Effectivity { get; }
    }
}
