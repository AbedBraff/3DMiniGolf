using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuttingScript : MonoBehaviour {

	public float minPuttForce = 0f;
	public float maxPuttForce = 30.0f;
	public float maxPuttChargeTime = .75f;
	public Slider puttForceSlider;
	public Image fillImage;
	public Camera mainCam;
	public float stoppingSpeed;


	private Rigidbody ballRb;
	private float currentPuttForce;
	private float chargeSpeed;
	private bool hasPutted;
	private Color minPuttForceColor = Color.yellow;
	private Color midPuttForceColor = Color.green;
	private Color maxPuttForceColor = Color.red;
	private float halfPuttForce;


	private void Awake()
	{
		ballRb = GetComponent<Rigidbody> ();
		puttForceSlider.maxValue = maxPuttForce;

		chargeSpeed = (maxPuttForce - minPuttForce) / maxPuttChargeTime;
		halfPuttForce = maxPuttForce / 2.0f;
	}


	private void OnEnable()
	{
		currentPuttForce = minPuttForce;
		puttForceSlider.value = minPuttForce;
	}


	void Update()
	{
		if (!hasPutted)
		{
			if (ShouldPutt ())
				Putt ();
		}
	}


	void FixedUpdate()
	{
		/* 
		 * What to do when the ball has been putted and is still in motion
		 */
		if (hasPutted && ballRb.velocity != Vector3.zero)
		{
			//	Check to see if the ball is going slow enough to auto-stop it
			if (ballRb.velocity.magnitude < stoppingSpeed)
			{
				ballRb.velocity = Vector3.zero;
				hasPutted = false;
			}
		}
	}


	/*
	 * Determine if the ball should be struck based purely on the current level of the hit meter
	 */
	private bool ShouldPutt()
	{
		puttForceSlider.value = minPuttForce;
		fillImage.color = minPuttForceColor;

		if (GetVelocity () == Vector3.zero)
		{
			if (currentPuttForce >= maxPuttForce && !hasPutted)
			{
				//	max charge, not putted
				currentPuttForce = maxPuttForce;
				return true;
			}
			else if (Input.GetKeyDown (KeyCode.Space))
			{
				//	pressed space first time
				//hasPutted = false;
				currentPuttForce = minPuttForce;
			}
			else if (Input.GetKey (KeyCode.Space) && !hasPutted)
			{
				//	holding putt, not fired
				currentPuttForce += chargeSpeed * Time.deltaTime;

				puttForceSlider.value = currentPuttForce;

				if (currentPuttForce < halfPuttForce)
				{
					// putt force is from min to half
					fillImage.color = Color.Lerp(minPuttForceColor, midPuttForceColor, (currentPuttForce / halfPuttForce));
				}
				else if (currentPuttForce > halfPuttForce)
				{
					//	putt force is from half to max
					fillImage.color = Color.Lerp(midPuttForceColor, maxPuttForceColor, ((currentPuttForce - halfPuttForce) / halfPuttForce));
				}
			}
			else if (Input.GetKeyUp (KeyCode.Space) && !hasPutted)
			{
				//	released space, not fired
				return true;
			}
		}

		return false;
	}


	private void Putt()
	{
		Vector3 puttVector = (mainCam.transform.forward);
		puttVector.y = 0f;
		puttVector.Normalize ();
		puttVector *= currentPuttForce;

		ballRb.AddForce (puttVector, ForceMode.Impulse);

		hasPutted = true;
	}


	Vector3 GetVelocity()
	{
		return ballRb.velocity;
	}
}
