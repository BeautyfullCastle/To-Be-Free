using UnityEngine;

public class ResolutionDetection : MonoBehaviour
{
    UIPopupList PopupList;
    UIToggle toggle;

    void Start()
    {
        PopupList = GetComponent<UIPopupList>();
        toggle = transform.parent.GetComponentInChildren<UIToggle>();
        Resolution[] resolutions = Screen.resolutions;
        foreach (Resolution res in resolutions)
        {
            PopupList.items.Add(res.width + "x" + res.height);
        }
    }

    public void OnSelectionChange(string selectedItem)
    {
        string[] resolution = selectedItem.Split('x');

        Screen.SetResolution(int.Parse(resolution[0]), int.Parse(resolution[1]), toggle.value);
    }

    public void ToggleFullScreen(bool isFull)
    {
        Screen.fullScreen = isFull;
    }
}