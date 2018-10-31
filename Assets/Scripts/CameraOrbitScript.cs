/*
 * Zachary Mitchell - 10/12/2018 - 3DMiniGolfwithNoFriends
 */

using UnityEngine;
using System.Collections;

public class CameraOrbitScript : MonoBehaviour {

	public Transform target;
	public float distance = 5.0f;
	public float xSpeed = 120.0f;
	public Light directionalLight;


	private Quaternion rotation;
	private float x = 0.0f;
	private float y = 0.0f;
	private PuttingScript puttingScript;
	private Quaternion lightRotationOffset;


	private const string playerTag = "Player";


	private void Start () 
	{
		//	Save starting rotation for x and y (z will always be 0; no fancy movie camera rotations here)
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;

		lightRotationOffset = directionalLight.transform.rotation;

		puttingScript = GameObject.FindGameObjectWithTag (playerTag).GetComponentInChildren<PuttingScript> ();
		if (!puttingScript)
		{
			print ("Error: Cannot find PuttingScript.");
		}
	}

	private void LateUpdate () 
	{
		//	Only allow camera control when the ball is moving, or the player isn't currently using the putt meter
		if (!puttingScript.isActiveAndEnabled || puttingScript.GetCurrentPuttForce() == 0)
			SetRotation ();
		
		SetPosition ();
	}


	private void SetRotation()
	{
		if (target)
		{
			x += Input.GetAxis ("Mouse X") * xSpeed * distance * Time.deltaTime;

			rotation = Quaternion.Euler(y, x, 0);
			transform.rotation = rotation;

			directionalLight.transform.rotation = rotation * lightRotationOffset;
		}
	}


	private void SetPosition()
	{
		Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
		Vector3 position = rotation * negDistance + target.position;

		transform.position = position;
	}
}