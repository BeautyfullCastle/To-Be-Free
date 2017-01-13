﻿using System;
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
		private float hour;
		private int day;
		private int policeTurnDays;

		private float totalHour;
		private float usedHour;
		private float timePerHour;
		private AudioSource hourAudioSource;

		public delegate void TimeEventHandler();

		public event TimeEventHandler NotifyEveryHour = delegate { };
		public event TimeEventHandler NotifyEveryday = delegate { };
		public event TimeEventHandler NotifyEveryWeek = delegate { };

		public TimeTable()
		{
			timePerHour = 0.3f;
			policeTurnDays = GameManager.Instance.PoliceTurnDays;
			hourAudioSource = AudioManager.Instance.Find("hour");

			Reset();
		}

		public void Reset()
		{
			Hour = 6;
			Day = 1;
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

			if (day % policeTurnDays == 0)
			{
				NotifyEveryWeek();
			}

			Debug.Log("Alived day : " + day);
		}

		public IEnumerator SpendTime(int requiredTime, eSpendTime timer)
		{
			totalHour = (requiredTime) * 6;
			if(timer == eSpendTime.RAND)
			{
				totalHour = UnityEngine.Random.Range(0, totalHour);
			}
			usedHour = 0f;

			yield return TurningHour();
		}

		public IEnumerator SpendRemainTime()
		{
			yield return TurningHour();
		}

		private IEnumerator TurningHour()
		{
			while (usedHour <= totalHour)
			{
				yield return new WaitForEndOfFrame();
				Hour += Time.deltaTime / TimeTable.Instance.TimePerHour;
				usedHour += Time.deltaTime / TimeTable.Instance.TimePerHour;
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

		public float Hour
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

				if(hourAudioSource == null)
				{
					hourAudioSource = AudioManager.Instance.Find("hour");
				}
				else if (hour % 1 <= 0.1f && hourAudioSource.isPlaying == false)
				{
					hourAudioSource.Play();
				}
				
				NotifyEveryHour();
			}
		}

		public int DDay
		{
			get
			{
				if(policeTurnDays <= 0)
				{
					return -1;
				}
				return policeTurnDays - (day % policeTurnDays);
			}
		}

		public float MoveTimePerAction
		{
			get
			{
				return timePerHour * 6;
			}
		}

		public float TimePerHour
		{
			get
			{
				return timePerHour;
			}
		}
	}
}