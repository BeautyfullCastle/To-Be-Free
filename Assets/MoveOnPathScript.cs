using UnityEngine;
using System.Collections;

public class MoveOnPathScript : MonoBehaviour {

	public EditorPathScript PathToFollow;

	public int CurrentWayPointID = 0;
	public float speed;
	public float rotationSpeed = 5.0f;
	public string pathName;

	private float reachDistance = 0.1f;

	private Vector3 last_position;
	private Vector3 current_position;

	void Start ()
	{
		//PathToFollow = GameObject.Find(pathName).GetComponent<EditorPathScript>();

		last_position = transform.position;
	}
	
	void Update ()
	{
		current_position = PathToFollow.path_objs[CurrentWayPointID].position;
		float distance = Vector3.Distance(current_position, transform.position);
		transform.position = Vector3.MoveTowards(transform.position, current_position, Time.deltaTime * speed);

		//var rotation = Quaternion.LookRotation(PathToFollow.path_objs[CurrentWayPointID].position - transform.position);
		//transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);


		if(distance <= reachDistance)
		{
			CurrentWayPointID--;
		}

		if (CurrentWayPointID >= PathToFollow.path_objs.Count)
		{
			CurrentWayPointID = 0;
		}
			
	}
}
