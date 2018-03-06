namespace PriceManagement.Domain.Membership
{
    public class Member
    {
        public int Id { get; }

        public MemberReference Parent { get; }

        public string Name { get; }

        public string ExternalCode { get; }

        public MemberOrganizationLevel Level { get; }
    }
}
