using UnityEngine;
using System.Collections;
using ToBeFree;

public class UICommand : MonoBehaviour {

	private int skippedDay;

	// Use this for initialization
	void Awake () {
		Effect.SkipEvent += Effect_SkipEvent;
		TimeTable.Instance.NotifyEveryday += Instance_NotifyEveryday;
	}

	private void Instance_NotifyEveryday()
	{
		skippedDay++;
		if(skippedDay >= 2)
		{
			this.gameObject.SetActive(true);
		}
	}

	private void Effect_SkipEvent(string eventType)
	{
		if(this.name == eventType)
		{
			skippedDay = 0;
			this.gameObject.SetActive(false);
		}
	}

}
