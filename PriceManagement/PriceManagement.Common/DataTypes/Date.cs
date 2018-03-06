using System;

namespace PriceManagement.Common.DataTypes
{
    public class Date : IEquatable<Date>, IComparable<Date>, IComparable
    {
        public static Date Today => new Date(DateTime.Today);

        public static Date FromDateTime(DateTime dateTime)
        {
            return new Date(dateTime.Date);
        }

        private readonly DateTime _datetime;

        private Date(DateTime datetime)
        {
            this._datetime = datetime;
        }

        public bool Equals(Date other)
        {
            return _datetime.Equals(other._datetime);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Date && Equals((Date) obj);
        }

        public override int GetHashCode()
        {
            return _datetime.GetHashCode();
        }

        public static bool operator ==(Date left, Date right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Date left, Date right)
        {
            return !left.Equals(right);
        }

        public int CompareTo(Date other)
        {
            return _datetime.CompareTo(other._datetime);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            if (!(obj is Date)) throw new ArgumentException($"Object must be of type {nameof(Date)}");
            return CompareTo((Date) obj);
        }

        public static bool operator <(Date left, Date right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(Date left, Date right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(Date left, Date right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(Date left, Date right)
        {
            return left.CompareTo(right) >= 0;
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
