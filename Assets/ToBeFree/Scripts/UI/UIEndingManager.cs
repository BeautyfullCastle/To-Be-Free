using System;
using System.Collections;
using UnityEngine;

namespace ToBeFree
{
	public enum eEnding
	{
		STARVATION = 0, REPATRIATE, HAPPY, END
	}

	public class UIEndingManager : MonoBehaviour
	{
		public UITexture page;

		// HP=0, 북송, 해피엔딩
		public Texture[] starvationTextures;
		public Texture[] repatriateTextures;
		public Texture[] happyTextures;

		void Awake()
		{
			Stat.OnValueChange += Stat_OnValueChange;
		}

		private void Stat_OnValueChange(int value, eStat stat)
		{
			if(stat == eStat.HP && value <= 0)
			{
				GameManager.Instance.ChangeScene(GameManager.eSceneState.Ending);
				StartCoroutine(this.StartEnding(eEnding.STARVATION));
			}
		}

		public IEnumerator StartEnding(eEnding ending)
		{
			GameManager.Instance.ChangeScene(GameManager.eSceneState.Ending);

			switch(ending)
			{
				case eEnding.STARVATION:
					yield return TurnPages(starvationTextures);
					break;
				case eEnding.REPATRIATE:
					yield return TurnPages(repatriateTextures);
					break;
				case eEnding.HAPPY:
					yield return TurnPages(happyTextures);
					break;
			}

			GameManager.Instance.ChangeScene(GameManager.eSceneState.Main);
		}

		private IEnumerator TurnPages(Texture[] textures)
		{
			foreach(Texture texture in textures)
			{
				TweenColor.Begin(this.gameObject, 1f, Color.black);
				yield return new WaitForSeconds(1f);

				page.mainTexture = texture;

				TweenColor.Begin(this.gameObject, 1f, Color.white);
				yield return new WaitForSeconds(1f);
			}
		}
	}
}