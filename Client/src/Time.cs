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
		public String AccuracyTime{ get;}
		public static String now() {
			String year = (DateTime.Now.Year - 1900).ToString();
			String day = DateTime.Now.DayOfYear.ToString();
			String clock = DateTime.Now.ToLongTimeString().Replace(":", "-");
			return year + "-" + day + "-" + clock;
		}
	}

}
