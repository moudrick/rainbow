using System;

namespace Rainbow.Data.Configuration
{
	/// <summary>
	/// Summary description for ConfigScheduler.
	/// </summary>
	public class ConfigScheduler
	{
		public ConfigScheduler()
		{
		}

		private bool schedulerEnabled;
		public bool SchedulerEnabled
		{
			get
			{
				return schedulerEnabled;
			}
			set
			{
				schedulerEnabled = value;
                
			}
		}

		private int schedulerCacheSize = 50;
		public int SchedulerCacheSize
		{
			get
			{
				return schedulerCacheSize;
			}
			set
			{
				schedulerCacheSize = value;
                
			}
		}

		private int schedulerPeriod = 60000;
		public int SchedulerPeriod
		{
			get
			{
				return schedulerPeriod;
			}
			set
			{
				schedulerPeriod = value;
                
			}
		}

	}
}
