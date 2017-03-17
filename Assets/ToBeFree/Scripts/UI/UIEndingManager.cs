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
		[SerializeField]
		private UITexture page;

		// HP=0, 북송, 해피엔딩
		public Texture[] starvationTextures;
		public Texture[] repatriateTextures;
		public Texture[] happyTextures;

		void Awake()
		{
			Stat.OnValueChange += Stat_OnValueChange;
			this.page = this.GetComponentInChildren<UITexture>();
		}

		private void Stat_OnValueChange(int value, eStat stat)
		{
			if(stat == eStat.HP && value <= 0)
			{
				this.gameObject.SetActive(true);
				StartCoroutine(this.StartEnding(eEnding.STARVATION));
			}
		}

		public IEnumerator StartEnding(eEnding ending)
		{
			yield return GameManager.Instance.ChangeScene(GameManager.eSceneState.Ending);

			AudioSource audio = null;
			switch(ending)
			{
				case eEnding.STARVATION:
					AudioManager.Instance.ChangeBGM("GameOver");
					yield return TurnPages(starvationTextures);
					break;
				case eEnding.REPATRIATE:
					AudioManager.Instance.ChangeBGM("GameOver");
					yield return TurnPages(repatriateTextures);
					break;
				case eEnding.HAPPY:
					AudioManager.Instance.ChangeBGM("HappyEnding");
					yield return TurnPages(happyTextures);
					break;
			}
			
			yield return GameManager.Instance.ChangeToMain();
		}

		private IEnumerator TurnPages(Texture[] textures)
		{
			foreach(Texture texture in textures)
			{
				TweenColor.Begin(this.gameObject, 1f, Color.black);
				yield return new WaitForSeconds(1f);

				if(page != null)
				{
					page.mainTexture = texture;
				}

				TweenColor.Begin(this.gameObject, 1f, Color.white);
				yield return new WaitForSeconds(1f);
			}
		}
	}
}