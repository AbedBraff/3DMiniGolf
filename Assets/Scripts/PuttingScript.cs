/*
 * Zachary Mitchell - 10/13/18 - 3DMinigolfwithNoFriends
 */

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


	private Rigidbody ballRigidBody;
	private float currentPuttForce;
	private float chargeSpeed;
	private Color minPuttForceColor = Color.yellow;
	private Color midPuttForceColor = Color.green;
	private Color maxPuttForceColor = Color.red;
	private float halfPuttForce;
	private bool willPutt;
	private Vector3 puttVector;
	private BallMovingScript ballMovingScript;


	private void Awake()
	{
		ballRigidBody = GetComponent<Rigidbody> ();
		ballMovingScript = GetComponent<BallMovingScript> ();

		puttForceSlider.maxValue = maxPuttForce;
		chargeSpeed = (maxPuttForce - minPuttForce) / maxPuttChargeTime;
		halfPuttForce = maxPuttForce / 2.0f;
		willPutt = false;
	}


	private void OnEnable()
	{
		currentPuttForce = minPuttForce;
		puttForceSlider.value = minPuttForce;
		willPutt = false;
	}


	void Update()
	{
		if (willPutt = ShouldPutt ())
			DeterminePuttVector ();
	}


	void FixedUpdate()
	{
		if (willPutt)
		{
			Putt ();
			TransferControl ();
		}
	}


	//	Determine if the ball should be struck based purely on the current level of the hit meter
	private bool ShouldPutt()
	{
		puttForceSlider.value = minPuttForce;
		fillImage.color = minPuttForceColor;

		if (currentPuttForce >= maxPuttForce)
		{
			//	max charge, not yet putted
			currentPuttForce = maxPuttForce;
			return true;
		}
		else if (Input.GetKeyDown (KeyCode.Space))
		{
			//	pressed space first time
			currentPuttForce = minPuttForce;
		}
		else if (Input.GetKey (KeyCode.Space))
		{
			//	holding putt, not fired
			currentPuttForce += chargeSpeed * Time.deltaTime;


			//	Update slider value and color according to currentPuttForce
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
		else if (Input.GetKeyUp (KeyCode.Space))
		{
			//	released space, not fired
			return true;
		}

		return false;
	}


	private void DeterminePuttVector()
	{
		puttVector = (mainCam.transform.forward);
		puttVector.y = 0f;
		puttVector.Normalize ();
		puttVector *= currentPuttForce;
	}


	private void Putt()
	{
		ballRigidBody.AddForce (puttVector, ForceMode.Impulse);
	}


	public void TransferControl()
	{
		ballMovingScript.enabled = true;
		this.enabled = false;
	}
}
