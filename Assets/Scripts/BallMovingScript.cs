/*
 * Zachary Mitchell - 10/15/18 - 3dMinigolfwithNoFriends
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovingScript : MonoBehaviour {


	public float stoppingSpeed = .25f;


	private PuttingScript puttingScript;
	private Rigidbody ballRigidBody;


	private void Awake()
	{
		puttingScript = GetComponent<PuttingScript> ();
		ballRigidBody = GetComponent<Rigidbody> ();
	}


	private void FixedUpdate()
	{
		if (ballRigidBody.velocity.magnitude <= stoppingSpeed)
		{
			StopBall ();
			TransferControl ();
		}
	}


	private void StopBall()
	{
		//	Set velocity to zero and freeze/unfreeze rotation so rotational inertia doesn't continue to move the ball
		ballRigidBody.velocity = Vector3.zero;
		ballRigidBody.freezeRotation = true;
		ballRigidBody.freezeRotation = false;
	}


	public void TransferControl()
	{
		puttingScript.enabled = true;
		this.enabled = false;
	}
}
