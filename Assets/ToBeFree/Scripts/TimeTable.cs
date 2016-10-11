using System.Collections;
using UnityEngine;

namespace ToBeFree
{
	public enum eSpendTime
	{
		RAND, END
	}

	public class TimeTable : Singleton<TimeTable>
	{

		private int hour;
		private int day;
		private readonly int week;

		int totalHour;
		int remainHour;

		public delegate void TimeEventHandler();

		public event TimeEventHandler NotifyEveryHour;
		public event TimeEventHandler NotifyEveryday;
		public event TimeEventHandler NotifyEveryWeek;

		public TimeTable()
		{
			hour = 6;
			day = 1;
			week = 7;
		}

		public void DayIsGone()
		{
			++day;

			NotifyEveryday();

			if (day % week == 0)
			{
				Debug.Log((day / week) + " weeks are gone.");
				NotifyEveryWeek();
			}

			Debug.Log("Alived day : " + day);
		}

		public IEnumerator SpendTime(int requiredTime, eSpendTime timer)
		{
			totalHour = (requiredTime+1) * 6;
			int usedHour = 0;
			int randHour = UnityEngine.Random.Range(0, totalHour);
			remainHour = totalHour - randHour;

			while (true)
			{
				yield return new WaitForSeconds(1f);
				Hour++;
				usedHour++;

				if (timer == eSpendTime.END)
				{
					if (usedHour >= totalHour)
					{
						yield break;
					}
				}
				else if (timer == eSpendTime.RAND)
				{
					
					if (usedHour >= randHour)
					{
						yield break;
					}
				}
			}
		}

		public IEnumerator SpendRemainTime()
		{
			while (remainHour < totalHour)
			{
				yield return new WaitForSeconds(1f);
				Hour++;
				remainHour++;
			}
		}

		public int Day
		{
			get
			{
				return day;
			}
		}

		public int Hour
		{
			get
			{
				return hour;
			}
			set
			{
				hour = value;
				NotifyEveryHour();
			}
		}
	}
}