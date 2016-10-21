using UnityEngine;
using System.Collections;

public class CameraZoom : MonoBehaviour {
	public float zoomAmount = 10.0f;
	public float minSize = 1.0f;
	public float maxSize = 3.0f;

	private Camera cam;
	
	// Use this for initialization
	void Start ()
	{
		cam = this.transform.GetComponent<Camera>();
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
	}
}
