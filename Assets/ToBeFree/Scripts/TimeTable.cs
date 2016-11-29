using System;
using System.Collections;
using UnityEngine;

namespace ToBeFree
{
	public enum eSpendTime
	{
		RAND, END
	}

	[Serializable]
	public class TimeSaveData
	{
		public int day;
	}

	public class TimeTable : Singleton<TimeTable>
	{

		private int hour;
		private int day;
		private readonly int week;

		int totalHour;
		int usedHour;

		public delegate void TimeEventHandler();

		public event TimeEventHandler NotifyEveryHour = delegate { };
		public event TimeEventHandler NotifyEveryday = delegate { };
		public event TimeEventHandler NotifyEveryWeek = delegate { };

		public TimeTable()
		{
			Hour = 6;
			Day = 1;
			week = 3;
		}

		public void Save(TimeSaveData data)
		{
			data.day = this.Day;
		}

		public void Load(TimeSaveData data)
		{
			this.Day = data.day;
		}

		public void DayIsGone()
		{
			++Day;
			Hour = 6;

			if (day % week == 0)
			{
				Debug.Log((day / week) + " weeks are gone.");
				NotifyEveryWeek();
			}

			Debug.Log("Alived day : " + day);
		}

		public IEnumerator SpendTime(int requiredTime, eSpendTime timer)
		{
			totalHour = (requiredTime) * 6;
			usedHour = 0;
			int randHour = UnityEngine.Random.Range(0, totalHour);
			int remainHour = totalHour - randHour;

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
			while (true)
			{
				yield return new WaitForSeconds(1f);
				Hour++;
				usedHour++;

				if (usedHour >= totalHour)
				{
					yield break;
				}
			}
		}

		public int Day
		{
			get
			{
				return day;
			}
			private set
			{
				day = value;
				NotifyEveryday();
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
				if (value >= 24)
					hour = 0;
				else
					hour = value;
				AudioManager.Instance.Find("hour").Play();
				NotifyEveryHour();
			}
		}
	}
}