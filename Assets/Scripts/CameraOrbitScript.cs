/*
 * Zachary Mitchell - 10/12/2018 - 3DMiniGolfwithNoFriends
 */

using UnityEngine;
using System.Collections;

public class CameraOrbitScript : MonoBehaviour {

	public Transform target;
	public float distance = 5.0f;
	public float xSpeed = 120.0f;

	private Quaternion rotation;
	private float x = 0.0f;
	private float y = 0.0f;


	private void Start () 
	{
		//	Save starting rotation for x and y (z will always be 0; no fancy movie camera rotations here)
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;
	}

	private void LateUpdate () 
	{
		SetRotation ();
		SetPosition ();
	}


	private void SetRotation()
	{
		if (target)
		{
			if (Input.GetKeyDown (KeyCode.RightArrow) || Input.GetKey(KeyCode.RightArrow))
				x -= xSpeed * distance * Time.deltaTime;
			if (Input.GetKeyDown (KeyCode.LeftArrow) || Input.GetKey (KeyCode.LeftArrow))
				x += xSpeed * distance * Time.deltaTime;

			rotation = Quaternion.Euler(y, x, 0);

			transform.rotation = rotation;
		}
	}


	private void SetPosition()
	{
		Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
		Vector3 position = rotation * negDistance + target.position;

		transform.position = position;
	}
}