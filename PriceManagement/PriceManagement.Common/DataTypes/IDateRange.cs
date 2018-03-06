namespace PriceManagement.Common.DataTypes
{
    public interface IDateRange<TDateType>
    {
        TDateType BeginDate { get; }

        TDateType EndDate { get; }

        bool IsValidWithinConstraints { get; }

        TDateType GetEffectiveBeginDate();

        TDateType GetEffectiveEndDate();

        bool Contains(TDateType date);

        bool Overlaps(IDateRange<TDateType> other);

        bool Encompases(IDateRange<TDateType> other);

        bool IsUnbounded { get; }
    }
}
