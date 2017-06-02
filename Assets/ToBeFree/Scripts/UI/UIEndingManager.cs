using System;
using System.Collections;
using UnityEngine;

namespace ToBeFree
{
	public enum eEnding
	{
		STARVATION = 0, REPATRIATE, HAPPY, END
	}

	public enum eEndingEffect
	{
		AUDIO,
		FADE_IN, FADE_OUT,
		FADE_COLOR_WHITE, FADE_COLOR_BLACK,
		MOVE,
		ZOOM_IN,
	}

	[Serializable]
	public class UIEnding
	{
		[SerializeField]
		private eEndingEffect effect;
		[SerializeField]
		private UIEndingComponent component;
		[SerializeField]
		private float startTime;
		[SerializeField]
		private float endTime;
		
		public void Init()
		{
			if (component == null)
				return;

			component.Init();
		}

		public IEnumerator Play()
		{
			if (component == null)
				yield break;

			yield return component.Play(effect, startTime, endTime);
		}

		public float EndTime
		{
			get
			{
				return endTime;
			}
		}

	}

	public class UIEndingManager : MonoBehaviour
	{
		public UIEnding[] happyEndingArr;
		public UIEnding[] starvationEndingArr;
		public UIEnding[] repatriateEndingArr;

		void OnEnable()
		{
			foreach(UIEnding ending in happyEndingArr)
			{
				if (ending == null)
					continue;
				ending.Init();
			}
			foreach (UIEnding ending in starvationEndingArr)
			{
				if (ending == null)
					continue;
				ending.Init();
			}
			foreach (UIEnding ending in repatriateEndingArr)
			{
				if (ending == null)
					continue;
				ending.Init();
			}
		}

		public IEnumerator StartEnding(eEnding ending)
		{
			yield return GameManager.Instance.ChangeScene(GameManager.eSceneState.Ending);
			
			switch(ending)
			{
				case eEnding.STARVATION:
					yield return Play(starvationEndingArr);
					break;
				case eEnding.REPATRIATE:
					yield return Play(repatriateEndingArr);
					break;
				case eEnding.HAPPY:
					yield return Play(happyEndingArr);
					break;
			}

			SaveLoadManager.Instance.Delete();
			yield return (GameManager.Instance.ChangeToMain());
		}

		private IEnumerator Play(UIEnding[] endings)
		{
			if (endings.Length <= 0)
				yield break;

			Debug.LogWarning("ending start time : " + Time.time);

			foreach (UIEnding ending in endings)
			{
				StartCoroutine(ending.Play());
			}
			yield return new WaitForSeconds(endings[endings.Length - 1].EndTime);
			Debug.LogWarning("ending end time : " + Time.time);
		}
	}
}