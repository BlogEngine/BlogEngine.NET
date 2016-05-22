using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace BlogEngine.Core
{
	public static class Persian
	{
		public static string GetPostDateInCurrentCulture(this DateTime date)
		{
			string str = "";
			str = (BlogSettings.Instance.Culture == "fa" ? date.ToPersianDate("/") : date.ToString("d. MMMM yyyy"));
			return str;
		}

		public static string ToPersianDate(this DateTime date, string seperator)
		{
			PersianCalendar persianCalendar = new PersianCalendar();
			StringBuilder stringBuilder = new StringBuilder();
			int year = persianCalendar.GetYear(date);
			stringBuilder.Append(year.ToString("0000"));
			stringBuilder.Append(seperator);
			int month = persianCalendar.GetMonth(date);
			stringBuilder.Append(month.ToString("00"));
			stringBuilder.Append(seperator);
			int dayOfMonth = persianCalendar.GetDayOfMonth(date);
			stringBuilder.Append(dayOfMonth.ToString("00"));
			return stringBuilder.ToString();
		}

		public static string ToPersianDateByFirstDayOfMonth(this DateTime date, string seperator)
		{
			PersianCalendar persianCalendar = new PersianCalendar();
			StringBuilder stringBuilder = new StringBuilder();
			int year = persianCalendar.GetYear(date);
			stringBuilder.Append(year.ToString("0000"));
			stringBuilder.Append(seperator);
			int month = persianCalendar.GetMonth(date);
			stringBuilder.Append(month.ToString("00"));
			stringBuilder.Append(seperator);
			stringBuilder.Append("01");
			return stringBuilder.ToString();
		}

		public static string ToPersianDateTime(this DateTime date)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(date.ToPersianFullName());
			stringBuilder.Append(" ساعت ");
			string str = date.Hour.ToString("00");
			int minute = date.Minute;
			stringBuilder.Append(string.Concat(str, ":", minute.ToString("00")));
			return stringBuilder.ToString();
		}

		public static int ToPersianDayOfMonth(this DateTime date)
		{
			return (new PersianCalendar()).GetDayOfMonth(date);
		}

		public static string ToPersianFullName(this DateTime date)
		{
			PersianCalendar persianCalendar = new PersianCalendar();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(date.ToPersianWeekDayName());
			stringBuilder.Append(" ");
			stringBuilder.Append(persianCalendar.GetDayOfMonth(date));
			stringBuilder.Append(" ");
			stringBuilder.Append(date.ToPersianMonthName());
			stringBuilder.Append(" ");
			stringBuilder.Append(persianCalendar.GetYear(date));
			return stringBuilder.ToString();
		}
        public static string ToPersianFullDateTime(this DateTime date)
        {
            PersianCalendar persianCalendar = new PersianCalendar();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(date.ToString("HH:mm"));
            stringBuilder.Append(" ");
            stringBuilder.Append(date.ToPersianWeekDayName());
            stringBuilder.Append(" ");
            stringBuilder.Append(persianCalendar.GetDayOfMonth(date));
            stringBuilder.Append(" ");
            stringBuilder.Append(date.ToPersianMonthName());
            stringBuilder.Append(" ");
            stringBuilder.Append(persianCalendar.GetYear(date));
            return stringBuilder.ToString();
        }
        public static string ToPersianMonthName(this DateTime date)
		{
			int month = (new PersianCalendar()).GetMonth(date);
			string str = "";
			switch (month)
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

		public static int ToPersianMonthNumber(this DateTime date)
		{
			return (new PersianCalendar()).GetMonth(date);
		}

		public static string ToPersianWeekDayName(this DateTime date)
		{
			DayOfWeek dayOfWeek = (new PersianCalendar()).GetDayOfWeek(date);
			string str = "";
			switch (dayOfWeek)
			{
				case DayOfWeek.Sunday:
				{
					str = "یکشنبه";
					break;
				}
				case DayOfWeek.Monday:
				{
					str = "دوشنبه";
					break;
				}
				case DayOfWeek.Tuesday:
				{
					str = "سه شنبه";
					break;
				}
				case DayOfWeek.Wednesday:
				{
					str = "چهار شنبه";
					break;
				}
				case DayOfWeek.Thursday:
				{
					str = "پنج شنبه";
					break;
				}
				case DayOfWeek.Friday:
				{
					str = "جمعه";
					break;
				}
				case DayOfWeek.Saturday:
				{
					str = "شنبه";
					break;
				}
			}
			return str;
		}

		public static int ToPersianYear(this DateTime date)
		{
			return (new PersianCalendar()).GetYear(date);
		}

		public static string ToPersianYearMonthName(this DateTime date)
		{
			PersianCalendar persianCalendar = new PersianCalendar();
			int month = persianCalendar.GetMonth(date);
			string str = "";
			switch (month)
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
			str = string.Concat(str, " ", persianCalendar.GetYear(date));
			return str;
		}
	}
}