using UnityEngine;
using System.Collections;

public class UIZoomTexture : MonoBehaviour {
	public int zoomAmount = 10;
	private UIWidget[] uiWidgets;
	private int width;
	private int height;

	// Use this for initialization
	void Start () {
		uiWidgets = this.transform.GetComponentsInChildren<UIWidget>();

		width = 0;
		height = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			width = zoomAmount;
			height = zoomAmount;
		}
		else if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			width = -zoomAmount;
			height = -zoomAmount;
		}

		if (Input.GetAxis("Mouse ScrollWheel") != 0)
		{
			foreach (UIWidget widget in uiWidgets)
			{
				widget.width += width;
				widget.height += height;
			}
		}

	}
}
