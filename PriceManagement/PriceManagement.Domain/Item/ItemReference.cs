using System;

namespace PriceManagement.Domain.Item
{
    public class ItemReference : Reference<string, int, Item>
    {
        public const string ReferenceType = "Item";

        public ItemReference(int id, string header, Item payload) : base(ReferenceType, id, header, payload)
        {
        }

        public ItemReference(int id, string header, Func<Item> payloadGenerator) : base(ReferenceType, id, header, payloadGenerator)
        {
        }
    }
}
