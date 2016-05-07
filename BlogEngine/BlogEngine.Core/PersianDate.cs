using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BlogEngine.Core
{
	public class PersianDate : IComparable
    {
		public int Day
		{
			get;
			set;
		}

		public int Month
		{
			get;
			set;
		}

		public int Year
		{
			get;
			set;
		}

		public PersianDate()
		{
		}

		public PersianDate(int year, int month, int day)
		{
			this.Year = year;
			this.Month = month;
			this.Day = day;
		}

	    public PersianDate(DateTime georgianDate)
	    {
	        var pc=new PersianCalendar();
	        Year = pc.GetYear(georgianDate);
            Month = pc.GetMonth(georgianDate);
            Day = pc.GetDayOfMonth(georgianDate);
        }

        public PersianDate AddMonths(int month)
		{
			PersianDate persianDate = new PersianDate(this.Year, this.Month, this.Day);
			PersianDate persianDate1 = persianDate;
			persianDate1.Month = persianDate1.Month + month;
			return persianDate;
		}
        public PersianDate AddYears(int years)
		{
			PersianDate persianDate = new PersianDate(this.Year, this.Month, this.Day);
			PersianDate year = persianDate;
			year.Year = year.Year + years;
			return persianDate;
		}

		public static PersianDate Parse(string s)
		{
			char[] chrArray = new char[] { '-' };
			int[] array = s.Split(chrArray).Select<string, int>(new Func<string, int>(int.Parse)).ToArray<int>();
			if ((int)array.Length != 3)
			{
				throw new FormatException("Invalid format.");
			}
			PersianDate persianDate = new PersianDate()
			{
				Year = array[0],
				Month = array[1]
			};
			return persianDate;
		}

		public static PersianDate Parse(string s, char seperator)
		{
			char[] chrArray = new char[] { seperator };
			int[] array = s.Split(chrArray).Select<string, int>(new Func<string, int>(int.Parse)).ToArray<int>();
			if ((int)array.Length != 3)
			{
				throw new FormatException("Invalid format.");
			}
			PersianDate persianDate = new PersianDate()
			{
				Year = array[0],
				Month = array[1],
				Day = array[2]
			};
			return persianDate;
		}

	    public DateTime ToDateTime()
		{
			PersianCalendar persianCalendar = new PersianCalendar();
			return persianCalendar.ToDateTime(this.Year, this.Month, this.Day, 0, 0, 0, 0);
		}

		public string ToPersianDateWithMonthName()
		{
			string str = this.Year.ToString();
			string persianMonthName = this.ToPersianMonthName();
			int day = this.Day;
			string str1 = string.Concat(str, " ", persianMonthName, day.ToString());
			return str1;
		}

		public string ToPersianMonthName()
		{
			string str = "";
			switch (this.Month)
			{
				case 1:
				{
					str = "فروردین";
					break;
				}
				case 2:
				{
					str = "اردیبهشت";
					break;
				}
				case 3:
				{
					str = "خرداد";
					break;
				}
				case 4:
				{
					str = "تیر";
					break;
				}
				case 5:
				{
					str = "مرداد";
					break;
				}
				case 6:
				{
					str = "شهریور";
					break;
				}
				case 7:
				{
					str = "مهر";
					break;
				}
				case 8:
				{
					str = "آبان";
					break;
				}
				case 9:
				{
					str = "آذر";
					break;
				}
				case 10:
				{
					str = "دی";
					break;
				}
				case 11:
				{
					str = "بهمن";
					break;
				}
				case 12:
				{
					str = "اسفند";
					break;
				}
			}
			return str;
		}

		public string ToStringYearMonthName()
		{
			string str = string.Concat(this.ToPersianMonthName(), " ", this.Year);
			return str;
		}

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            var y = obj as PersianDate;
            if (Year > y.Year || (Year == y.Year && Month > y.Month) ||
                (Year == y.Year && Month == y.Month && Day > y.Day))
            {
                return -1;
            }
                
            else if (Year < y.Year || (Year == y.Year && Month < y.Month) ||
                (Year == y.Year && Month == y.Month && Day < y.Day))
            {
                return 1;
            }
            else if (Year == y.Year && Month == y.Month && Day == y.Day)
            {
                return 0;
            }
            else
            {
                throw new ArgumentException("Object is not a PersianDate");
            }
        }
    }
}