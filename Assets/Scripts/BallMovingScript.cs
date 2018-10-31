/*
 * Zachary Mitchell - 10/15/18 - 3dMinigolfwithNoFriends
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovingScript : MonoBehaviour {


	public float stoppingSpeed = .25f;
	public AudioClip wallBounceClip;
	public AudioClip grassBounceClip;
	public AudioSource audioSource;
	public AudioSource rollingAudioSource;
	public float pitchRange;


	private float maxPuttForce;
	private PuttingScript puttingScript;
	private Rigidbody ballRigidBody;
	private float originalPitch;


	private const string wallTag = "Wall";
	private const string groundTag = "Ground";


	private void Awake()
	{
		puttingScript = GetComponent<PuttingScript> ();
		ballRigidBody = GetComponent<Rigidbody> ();
		audioSource = GetComponent<AudioSource> ();

		maxPuttForce = puttingScript.GetMaxPuttForce ();
		originalPitch = audioSource.pitch;
	}


	private void FixedUpdate()
	{
		if (ballRigidBody.velocity.magnitude <= stoppingSpeed)
		{
			StopBall ();
			TransferControl ();
		}
	}
		

	private void OnCollisionEnter(Collision collision)
	{
		//	Play audio for wall bounce if ball hits a wall with volume based on velocity
		if (collision.gameObject.CompareTag (wallTag))
		{
			audioSource.clip = wallBounceClip;
			audioSource.volume = ballRigidBody.velocity.magnitude / maxPuttForce;
			audioSource.Play ();
		}
		else if (collision.gameObject.CompareTag (groundTag))
		{
			audioSource.clip = grassBounceClip;
			audioSource.volume = (1f - (Mathf.Exp(-1f * ballRigidBody.velocity.magnitude)));
			audioSource.Play ();
		}
	}


	private void OnCollisionStay(Collision collision)
	{
		if (collision.gameObject.CompareTag (groundTag))
		{
			BallRollingAudio ();
		}
	}


	private void BallRollingAudio()
	{
		rollingAudioSource.Play ();
		rollingAudioSource.volume = ballRigidBody.velocity.magnitude / maxPuttForce;
		rollingAudioSource.pitch = Random.Range (originalPitch - pitchRange, originalPitch + pitchRange);
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
