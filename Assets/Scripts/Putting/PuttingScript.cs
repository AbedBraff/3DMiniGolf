﻿/*
 * Zachary Mitchell
 * 3DGolfwithNoFriends
 */


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PuttingScript : MonoBehaviour
{
    public AudioClip m_PuttClip;
    public float m_MaxPuttChargeTime = 2f;
	public float m_MaxPuttForce = 0.5f;


    private AudioSource m_PuttAudioSource;
	private Rigidbody m_BallRigidBody;
    private Color m_MinPuttForceColor = Color.yellow;
    private Color m_MidPuttForceColor = Color.green;
    private Color m_MaxPuttForceColor = Color.red;
    private Vector3 m_PuttVector;
    private BallMovingScript m_BallMovingScript;
    private PlayerManager m_PlayerManagerScript;
    private Slider m_PuttForceSlider;
    private Image m_FillImage;
    private float m_CurrentPuttForce;
	private float m_ChargeSpeed;
	private float m_HalfPuttForce;
	private bool m_WillPutt;
	private bool m_StartedPutting;
    

    private const float M_MINPUTTFORCE = 0f;
    private const string M_PUTTSLIDERNAME = "PuttForceSlider";


    //  Setters and getters
    public float GetCurrentPuttForce()
    {
        return m_CurrentPuttForce;
    }


    public void Setup()
	{
        //  Find and cache needed scripts and components
        try
        {
            m_PuttAudioSource = GetComponent<AudioSource>();
            m_BallRigidBody = GetComponent<Rigidbody>();
            m_BallMovingScript = GetComponent<BallMovingScript>();
            m_PlayerManagerScript = this.transform.parent.gameObject.GetComponent<PlayerManager>();
            m_PuttForceSlider = GameObject.Find(M_PUTTSLIDERNAME).GetComponent<Slider>();
            m_FillImage = m_PuttForceSlider.image;

        }
        catch (Exception e)
        {
            print("Error: " + e.ToString());
        }

        //  Set vars to starting values
        ResetVariables();
		m_ChargeSpeed = (m_MaxPuttForce - M_MINPUTTFORCE) / m_MaxPuttChargeTime;
		m_HalfPuttForce = m_MaxPuttForce / 2.0f;

        this.enabled = false;
	}


    //  Called each time the user is ready to putt again - resets all needed variables to starting values
	private void OnEnable()
	{
        ResetVariables();
	}


    //  Reset vars to starting values
    private void ResetVariables()
    {
        m_PuttForceSlider.maxValue = m_MaxPuttForce;
        m_WillPutt = false;
        m_StartedPutting = false;
        m_PuttVector = Vector3.zero;
        m_CurrentPuttForce = 0.0f;
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
			TransferControlToBallMoving ();
		}
	}


	//	Determine if the ball should be struck based purely on the current level of the hit meter
	private bool ShouldPutt()
	{
		m_PuttForceSlider.value = M_MINPUTTFORCE;
		m_FillImage.color = m_MinPuttForceColor;

		if (m_CurrentPuttForce >= m_MaxPuttForce)
		{
			//	Max charge, not yet putted
			m_CurrentPuttForce = m_MaxPuttForce;
			return true;
		}
		else if (Input.GetKeyDown (KeyCode.Space))
		{
            //	Pressed space first time
			m_CurrentPuttForce = M_MINPUTTFORCE;
			m_StartedPutting = true;
		}
		else if (Input.GetKey (KeyCode.Space) && m_StartedPutting)
		{
			//	Holding putt, not fired
			m_CurrentPuttForce += m_ChargeSpeed * Time.deltaTime;


			//	Update slider value and color according to currentPuttForce
			m_PuttForceSlider.value = m_CurrentPuttForce;


            //  Putt slider color from 0 to half
			if (m_CurrentPuttForce < m_HalfPuttForce)
				m_FillImage.color = Color.Lerp(m_MinPuttForceColor, m_MidPuttForceColor, (m_CurrentPuttForce / m_HalfPuttForce));
            //  Putt slider color from half to full
			else if (m_CurrentPuttForce >= m_HalfPuttForce)
				m_FillImage.color = Color.Lerp(m_MidPuttForceColor, m_MaxPuttForceColor, ((m_CurrentPuttForce - m_HalfPuttForce)
									/ m_HalfPuttForce));
		}
		else if (Input.GetKeyUp (KeyCode.Space) && m_StartedPutting)
		{
            //	Released space, not yet fired 
            //  Also checks that we have started putting so that space being held down doesn't buffer the next shot when this script is enabled
			return true;
		}

		return false;
	}


	//	Calculate the force vector for the putt
	private void DeterminePuttVector()
	{
		m_PuttVector = m_BallRigidBody.transform.forward * m_CurrentPuttForce;
	}


	//	Putt the ball, play audio, and add a shot to the counter
	private void Putt()
	{
		PlayPuttAudio ();
		m_BallRigidBody.AddForce (m_PuttVector, ForceMode.Impulse);
        m_PlayerManagerScript.SetCurrentHoleShots(m_PlayerManagerScript.GetCurrentHoleShots() + 1);
	}


	//	Play audio for hitting the ball
	private void PlayPuttAudio()
	{
		m_PuttAudioSource.clip = m_PuttClip;
		m_PuttAudioSource.volume = m_CurrentPuttForce / (m_MaxPuttForce - M_MINPUTTFORCE);
		m_PuttAudioSource.Play ();
	}


	//	Transfer control to the BallMovingScript
	public void TransferControlToBallMoving()
	{
		m_BallMovingScript.enabled = true;
		this.enabled = false;
	}
}
