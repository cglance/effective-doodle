namespace PriceManagement.Common.DataTypes
{
    public static class DateExtensions
    {
        public static Date OrToday(this Date ctx)
        {
            return ctx ?? Date.Today;
        }

        public static UTCDate OrToday(this UTCDate ctx)
        {
            return ctx ?? UTCDate.Today;
        }
    }
}
