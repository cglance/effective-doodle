using System;

namespace PriceManagement.Domain.Membership
{
    public class MemberReference : Reference<string, int, Member>
    {
        public const string ReferenceType = "Member";

        public MemberReference(int id, string header, Member payload) : base(ReferenceType, id, header, payload)
        {
        }

        public MemberReference(int id, string header, Func<Member> payloadGenerator) : base(ReferenceType, id, header, payloadGenerator)
        {
        }
    }
}
