using UnityEngine;

public class ResolutionDetection : MonoBehaviour
{
	[SerializeField]
	private UIPopupList popupList;
	[SerializeField]
	UIToggle fullscreenCheckBox;

	void Start()
	{
		if(popupList == null)
		{
			popupList = GetComponent<UIPopupList>();
		}
		if(fullscreenCheckBox == null)
		{
			fullscreenCheckBox = transform.parent.parent.GetComponentInChildren<UIToggle>();
		}
		
		Resolution[] resolutions = Screen.resolutions;
		foreach (Resolution res in resolutions)
		{
			popupList.items.Add(res.width + "x" + res.height);
		}
		
		popupList.value = Screen.currentResolution.width + "x" + Screen.currentResolution.height;
	}

	public void OnSelectionChange(string selectedItem)
	{
		string[] resolution = selectedItem.Split('x');

		Screen.SetResolution(int.Parse(resolution[0]), int.Parse(resolution[1]), fullscreenCheckBox.value);
	}

	public void ToggleFullScreen(bool isFull)
	{
		Screen.fullScreen = isFull;
	}
}