using System;

namespace PriceManagement.Domain.Vendor
{
    public class VendorReference : Reference<string, int, Vendor>
    {
        public const string ReferenceType = "Vendor";

        public VendorReference(int id, string header, Vendor payload) : base(ReferenceType, id, header, payload)
        {
        }

        public VendorReference(int id, string header, Func<Vendor> payloadGenerator) : base(ReferenceType, id, header, payloadGenerator)
        {
        }
    }
}
