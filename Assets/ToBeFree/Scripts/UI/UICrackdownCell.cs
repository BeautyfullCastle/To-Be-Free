using UnityEngine;

public class UICrackdownCell : MonoBehaviour
{
	[SerializeField]
	private UISprite sprite;

	public void TurnOnSprite(bool isActive)
	{
		if (sprite == null)
			return;

		this.sprite.enabled = isActive;
	}
}
