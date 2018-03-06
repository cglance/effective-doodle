using System;

namespace PriceManagement.Domain.Contracting
{
    public class ContractReference : Reference<string, int, Contract>
    {
        public const string ReferenceType = "Contract";

        public ContractReference(int id, string header, Contract payload) : base(ReferenceType, id, header, payload)
        {
        }

        public ContractReference(int id, string header, Func<Contract> payloadGenerator) : base(ReferenceType, id, header, payloadGenerator)
        {
        }
    }
}
