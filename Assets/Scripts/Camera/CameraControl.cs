/*
 * Zachary Mitchell
 * 3DGolfwithNoFriends
 */


using UnityEngine;
using System.Collections;
using System;


public class CameraControl : MonoBehaviour {

	public float m_Distance = 0.75f;
	public float m_XSpeed = 120.0f;
    public float m_YSpeed = 90.0f;
    public float m_YMinAngle = 5.0f;
    public float m_YMaxAngle = 85.0f;
    public float m_MinDistance = .5f;
    public float m_MaxDistance = 2.5f;
    public float m_ZoomSpeed = 100.0f;
    public float m_KeyboardSpeedMultiplier = 1.5f;  //  Use for keyboard horizontal movement only


    private Transform m_Target;
    private Quaternion m_Rotation;
    private PuttingScript m_PuttingScript;
    private float m_CamAngleX = 0.0f;
	private float m_CamAngleY = 0.0f;


	private const string M_PLAYERTAG = "Player";
    private const string M_GAMEMANAGERTAG = "GameManager";


    //  Setters and getters
    public void SetTarget(Transform _value)
    {
        m_Target = _value;
    }


    public void Setup () 
	{
        //	Save starting rotation for x and y (z will always be 0; no fancy movie camera rotations here)
        Vector3 angles = transform.eulerAngles;
		m_CamAngleX = angles.y;
		m_CamAngleY = angles.x;


        //  Find and cache needed scripts and components
        try
        {
            m_PuttingScript = m_Target.gameObject.GetComponentInChildren<PuttingScript>(true);
        }
        catch (Exception e)
        {
            print("Error: " + e.ToString());
        }
	}


	private void LateUpdate () 
	{
		//	Only allow camera control when the ball is moving, or the player isn't currently using the putt meter
		if (!m_PuttingScript.isActiveAndEnabled || m_PuttingScript.GetCurrentPuttForce() == 0.0f)
			SetRotation ();
		
		SetPosition ();
	}


	private void SetRotation()
	{
        //  Make sure the script has a non-null target
		if (m_Target)
		{
            //  Mouse horizontal camera control
			if (Input.GetAxis ("Mouse X") != 0)
			{
				m_CamAngleX += Input.GetAxis ("Mouse X") * m_XSpeed * m_Distance * Time.deltaTime;
			}
            //  Keyboard horizontal camera control
			else
			{
				if(Input.GetKey(KeyCode.LeftArrow))
					m_CamAngleX += m_XSpeed * m_Distance * Time.deltaTime * m_KeyboardSpeedMultiplier;
				else if(Input.GetKey(KeyCode.RightArrow))
					m_CamAngleX -= m_XSpeed * m_Distance * Time.deltaTime * m_KeyboardSpeedMultiplier;
				
			}


            //  Mouse vertical camera control - clamped
            if (Input.GetAxis("Mouse Y") != 0)
            {
                m_CamAngleY += Input.GetAxis("Mouse Y") * m_YSpeed * m_Distance * Time.deltaTime;
                m_CamAngleY = Mathf.Clamp(m_CamAngleY, m_YMinAngle, m_YMaxAngle);
            }
            //  Keyboard vertical camera control - clamped
            else
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    m_CamAngleY += m_YSpeed * m_Distance * Time.deltaTime;
                    m_CamAngleY = Mathf.Clamp(m_CamAngleY, m_YMinAngle, m_YMaxAngle);
                }
                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    m_CamAngleY -= m_YSpeed * m_Distance * Time.deltaTime;
                    m_CamAngleY = Mathf.Clamp(m_CamAngleY, m_YMinAngle, m_YMaxAngle);
                }
            }

            //  Store and apply the new rotation to the camera
			m_Rotation = Quaternion.Euler(m_CamAngleY, m_CamAngleX, 0);
			transform.rotation = m_Rotation;


            //  Rotate the ball
			RotateBall ();
		}
	}


	//	Rotate the ball so that it's forward vector matches the camera; ie both have the same y rotation
	private void RotateBall()
	{
		Vector3 ballRotation = m_Rotation.eulerAngles;
		m_Target.rotation = Quaternion.Euler(0f, ballRotation.y, 0f);
	}


    //  Set the position of the camera
	private void SetPosition()
	{
        //  Mouse wheel zoom control - clamped
        if(Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            m_Distance -= Input.GetAxis("Mouse ScrollWheel") * m_ZoomSpeed * Time.deltaTime;
            m_Distance = Mathf.Clamp(m_Distance, m_MinDistance, m_MaxDistance);
        }


        //  Store and apply the new position to the camera
		Vector3 negDistance = new Vector3(0.0f, 0.0f, -m_Distance);
		Vector3 position = m_Rotation * negDistance + m_Target.position;

		transform.position = position;
	}
}