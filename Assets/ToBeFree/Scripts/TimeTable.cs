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

		public void Init()
		{
			timePerHour = GameManager.Instance.moveTimeSpeed;
			policeTurnDays = GameManager.Instance.PoliceTurnDays;
			if(hourAudioSource == null)
			{
				hourAudioSource = AudioManager.Instance.Find("hour");
			}
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
			float endHour = (requiredTime) * 6;
			if(timer == eSpendTime.RAND)
			{
				endHour = UnityEngine.Random.Range(0, totalHour);
			}
			usedHour = 0f;

			yield return TurningHour(endHour);
			totalHour = (requiredTime) * 6;
		}

		public IEnumerator SpendRemainTime()
		{
			yield return TurningHour(totalHour);
		}

		private IEnumerator TurningHour(float endHour)
		{
			while (usedHour <= endHour)
			{
				yield return new WaitForEndOfFrame();
				Hour += Time.deltaTime / TimePerHour;
				usedHour += Time.deltaTime / TimePerHour;
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
					if(GameManager.Instance.State != GameManager.GameState.Init)
					{
						hourAudioSource.Play();
					}
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