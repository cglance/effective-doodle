using PriceManagement.Domain.Vendor;

namespace PriceManagement.Domain.Item
{
    public class Item
    {
        public int Id { get; }

        public int UIN { get; }

        public VendorReference Manufacturer { get; }

        public string ManufacturerCatalogNumber { get; }

        public string Description { get; }
    }
}
