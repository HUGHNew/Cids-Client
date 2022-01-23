using System;

namespace Client
{
	public class Time
	{
		private String accuracyTime;
		public Time()
		{
			String year=(DateTime.Now.Year - 1900).ToString();
			String day = DateTime.Now.DayOfYear.ToString();
			String clock = DateTime.Now.ToLongTimeString().Replace(":", "-");
			accuracyTime = year + "-" + day + "-" + clock;
		}
		public String AccuracyTime => accuracyTime;
		public static String now() {
			String year = (DateTime.Now.Year - 1900).ToString();
			String day = DateTime.Now.DayOfYear.ToString();
			String clock = DateTime.Now.ToLongTimeString().Replace(":", "-");
			return year + "-" + day + "-" + clock;
		}
		//2022-01-20:21-14-03
		public static String FdNow() {
			return DateTime.Now.ToString("yyyy-MM-dd:HH-mm-ss");
		}
	}

}
