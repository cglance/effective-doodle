using System;
using System.Collections.Generic;
using System.Text;

namespace PriceManagement.Common.DataTypes
{
    public class BaseDateRange
    {
        private readonly DateRange[] _constraints;

        public Date BeginDate { get; }

        public Date EndDate { get; }

        public DateRange(Date beginDate, Date endDate)
        {
            if (beginDate > endDate) throw new ArgumentException("Negative Date Range");
            BeginDate = beginDate;
            EndDate = endDate;
        }

        public DateRange(Date beginDate, Date endDate, params DateRange[] constraints)
            : this(beginDate, endDate)
        {
            _constraints = constraints;
        }

        public bool IsValidWithinConstraints
        {
            get
            {
                if (_constraints != null)
                {
                    foreach (DateRange constraint in _constraints)
                    {
                        if (!constraint.Encompases(this))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }

        public Date GetEffectiveBeginDate()
        {
            var allDates = new List<Date>();
            if (BeginDate != null)
            {
                allDates.Add(BeginDate);
            }
            if (_constraints != null)
            {
                allDates.AddRange(_constraints.Select(c => c.GetEffectiveBeginDate()));
            }

            return allDates.Max();
        }

        public Date GetEffectiveEndDate()
        {
            var allDates = new List<Date>();
            if (EndDate != null)
            {
                allDates.Add(EndDate);
            }
            if (_constraints != null)
            {
                allDates.AddRange(_constraints.Select(c => c.GetEffectiveEndDate()));
            }

            return allDates.Min();
        }

        public bool Contains(Date date)
        {
            if (date == null) throw new ArgumentNullException(nameof(date));
            return (BeginDate == null || BeginDate <= date) && (EndDate == null || EndDate >= date);
        }

        public bool Overlaps(IDateRange<Date> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            return IsUnbounded ||
                   (other.BeginDate != null && Contains(other.BeginDate)) ||
                   (other.EndDate != null && Contains(other.EndDate));
        }

        public bool Encompases(IDateRange<Date> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            return (BeginDate == null || (other.BeginDate != null && Contains(other.BeginDate))) &&
                   (EndDate == null || (other.EndDate != null && Contains(other.EndDate)));
        }

        public bool IsUnbounded => BeginDate == null && EndDate == null;
    }
}
