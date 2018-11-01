/*
 * Zachary Mitchell - 10/13/18 - 3DMinigolfwithNoFriends
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuttingScript : MonoBehaviour {


	public float m_MaxPuttChargeTime = 2f;
	public Slider m_PuttForceSlider;
	public Image m_FillImage;
	public AudioClip m_PuttClip;
	public AudioSource m_PuttAudioSource;

	[SerializeField]private float m_MaxPuttForce = 0.5f;
	private Rigidbody m_BallRigidBody;
	private float m_CurrentPuttForce;
	private float m_ChargeSpeed;
	private Color m_MinPuttForceColor = Color.yellow;
	private Color m_MidPuttForceColor = Color.green;
	private Color m_MaxPuttForceColor = Color.red;
	private float m_HalfPuttForce;
	private bool m_WillPutt;
	private Vector3 m_PuttVector;
	private BallMovingScript m_BallMovingScript;
	private bool m_StartedPutting;


	private const float M_MINPUTTFORCE = 0f;


	private void Awake()
	{
		m_BallRigidBody = GetComponent<Rigidbody> ();
		m_BallMovingScript = GetComponent<BallMovingScript> ();

		m_PuttForceSlider.maxValue = m_MaxPuttForce;
		m_WillPutt = false;
		m_StartedPutting = false;

		m_ChargeSpeed = (m_MaxPuttForce - M_MINPUTTFORCE) / m_MaxPuttChargeTime;
		m_HalfPuttForce = m_MaxPuttForce / 2.0f;
	}


	private void OnEnable()
	{
		m_CurrentPuttForce = M_MINPUTTFORCE;
		m_PuttForceSlider.value = M_MINPUTTFORCE;
		m_WillPutt = false;
		m_StartedPutting = false;
	}


	void Update()
	{
		/*	
		 * Only check if we ShouldPutt if the ball hasn't been set to putt next
		 *	FixedUpdate.  Otherwise we will lose inputs.
		 *	If we will be putting, determine the putt vector for the next FixedUpdate
		*/
		if (!m_WillPutt)
		{
			if (m_WillPutt = ShouldPutt ())
				DeterminePuttVector ();
		}
	}


	void FixedUpdate()
	{
		//	Putt and transfer control to the BallMovingScript
		if (m_WillPutt)
		{
			Putt ();
			TransferControl ();
		}
	}


	//	Determine if the ball should be struck based purely on the current level of the hit meter
	private bool ShouldPutt()
	{
		m_PuttForceSlider.value = M_MINPUTTFORCE;
		m_FillImage.color = m_MinPuttForceColor;

		if (m_CurrentPuttForce >= m_MaxPuttForce)
		{
			//	max charge, not yet putted
			m_CurrentPuttForce = m_MaxPuttForce;
			return true;
		}
		else if (Input.GetKeyDown (KeyCode.Space))
		{
			//	pressed space first time
			m_CurrentPuttForce = M_MINPUTTFORCE;
			m_StartedPutting = true;
		}
		else if (Input.GetKey (KeyCode.Space) && m_StartedPutting)
		{
			//	holding putt, not fired
			m_CurrentPuttForce += m_ChargeSpeed * Time.deltaTime;


			//	Update slider value and color according to currentPuttForce
			m_PuttForceSlider.value = m_CurrentPuttForce;

			if (m_CurrentPuttForce < m_HalfPuttForce)
			{
				// putt force is from min to half
				m_FillImage.color = Color.Lerp(m_MinPuttForceColor, m_MidPuttForceColor, (m_CurrentPuttForce / m_HalfPuttForce));
			}
			else if (m_CurrentPuttForce > m_HalfPuttForce)
			{
				//	putt force is from half to max
				m_FillImage.color = Color.Lerp(m_MidPuttForceColor, m_MaxPuttForceColor, ((m_CurrentPuttForce - m_HalfPuttForce)
									/ m_HalfPuttForce));
			}
		}
		else if (Input.GetKeyUp (KeyCode.Space))
		{
			//	released space, not fired
			return true;
		}

		return false;
	}


	//	Calculate the force vector for the putt
	private void DeterminePuttVector()
	{
		m_PuttVector = m_BallRigidBody.transform.forward * m_CurrentPuttForce;
	}


	//	Putt the ball
	private void Putt()
	{
		PlayPuttAudio ();
		m_BallRigidBody.AddForce (m_PuttVector, ForceMode.Impulse);
	}


	//	Play audio for hitting the ball
	private void PlayPuttAudio()
	{
		m_PuttAudioSource.clip = m_PuttClip;
		m_PuttAudioSource.volume = m_CurrentPuttForce / (m_MaxPuttForce - M_MINPUTTFORCE);
		m_PuttAudioSource.Play ();
	}


	//	Transfer control to the BallMovingScript
	public void TransferControl()
	{
		m_BallMovingScript.enabled = true;
		this.enabled = false;
	}


	public float GetMaxPuttForce()
	{
		return m_MaxPuttForce;
	}


	public float GetCurrentPuttForce()
	{
		return m_CurrentPuttForce;
	}
}
