using UnityEngine;
using ToBeFree;

public class CameraZoom : MonoBehaviour {
	public float zoomAmount = 10.0f;
	public float minSize = 1.0f;
	public float maxSize = 3.0f;

	private Camera cam;
	private UIScrollView scrollview;
	private UICenterOnChild centerOnChild;
	
	// Use this for initialization
	void Start ()
	{
		cam = this.transform.GetComponent<Camera>();
		scrollview = this.transform.root.GetComponentInChildren<UIScrollView>();
		centerOnChild = scrollview.GetComponent<UICenterOnChild>();
	}

	public void Init()
	{
		this.cam.orthographicSize = minSize;
		CenterOnCamera(GameManager.Instance.Character.CurCity.IconCity.transform);
	}

	public void CenterOnCamera(Transform target)
	{
		this.centerOnChild.CenterOn(target);
		this.centerOnChild.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		float wheelAmount = Input.GetAxis("Mouse ScrollWheel");

		if (wheelAmount != 0)
		{
			cam.orthographicSize -= wheelAmount * zoomAmount;
			if (cam.orthographicSize < minSize)
			{
				cam.orthographicSize = minSize;
			}
			else if (cam.orthographicSize > maxSize)
			{
				cam.orthographicSize = maxSize;
			}
			
		}
		scrollview.RestrictWithinBounds(true);
	}
}
