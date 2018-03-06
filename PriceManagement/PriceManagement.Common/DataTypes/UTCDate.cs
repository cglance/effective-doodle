using System;
using System.Collections.Generic;

namespace PriceManagement.Common.DataTypes
{
    public class UTCDate
    {
        public static UTCDate Today => new UTCDate(DateTime.UtcNow.Date);

        public static UTCDate FromDateTime(DateTime datetime, bool treatUnspecifiedAsUTC = false)
        {
            if (datetime.Kind == DateTimeKind.Utc)
            {
                return new UTCDate(datetime.Date);
            }

            if (datetime.Kind == DateTimeKind.Unspecified && treatUnspecifiedAsUTC)
            {
                return new UTCDate(DateTime.SpecifyKind(datetime, DateTimeKind.Utc).Date);
            }

            if (datetime.Kind == DateTimeKind.Local)
            {
                return new UTCDate(datetime.ToUniversalTime().Date);
            }

            throw new ArgumentException("Presented datetime's kind is unspecified");
        }

        private readonly DateTime _datetime;

        private UTCDate(DateTime datetime)
        {
            if (datetime.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("Must provide a UTC datetime");
            }

            _datetime = datetime;
        }

        public bool Equals(UTCDate other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _datetime.Equals(other._datetime);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UTCDate)obj);
        }

        public override int GetHashCode()
        {
            return _datetime.GetHashCode();
        }

        public static bool operator ==(UTCDate left, UTCDate right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UTCDate left, UTCDate right)
        {
            return !Equals(left, right);
        }

        public int CompareTo(UTCDate other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return _datetime.CompareTo(other._datetime);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            if (ReferenceEquals(this, obj)) return 0;
            if (!(obj is UTCDate)) throw new ArgumentException($"Object must be of type {nameof(UTCDate)}");
            return CompareTo((UTCDate)obj);
        }

        public static bool operator <(UTCDate left, UTCDate right)
        {
            return Comparer<UTCDate>.Default.Compare(left, right) < 0;
        }

        public static bool operator >(UTCDate left, UTCDate right)
        {
            return Comparer<UTCDate>.Default.Compare(left, right) > 0;
        }

        public static bool operator <=(UTCDate left, UTCDate right)
        {
            return Comparer<UTCDate>.Default.Compare(left, right) <= 0;
        }

        public static bool operator >=(UTCDate left, UTCDate right)
        {
            return Comparer<UTCDate>.Default.Compare(left, right) >= 0;
        }

        public override string ToString()
        {
            return _datetime.ToString();
        }

        public string ToString(IFormatProvider provider)
        {
            return _datetime.ToString(provider);
        }

        public string ToString(string format)
        {
            return _datetime.ToString(format);
        }

        public string ToString(string format, IFormatProvider provider)
        {
            return _datetime.ToString(format, provider);
        }
    }
}
