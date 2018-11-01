/*
 * Zachary Mitchell - 10/12/2018 - 3DMiniGolfwithNoFriends
 */

using UnityEngine;
using System.Collections;

public class CameraOrbitScript : MonoBehaviour {

	public Transform m_Target;
	public float m_Distance = 0.75f;
	public float m_XSpeed = 120.0f;
	public Light m_DirectionalLight;


	private Quaternion m_Rotation;
	private float m_CamAngleX = 0.0f;
	private float m_CamAngleY = 0.0f;
	private PuttingScript m_PuttingScript;
	private Quaternion m_LightRotationOffset;


	private const string M_PLAYERTAG = "Player";


	private void Start () 
	{
		//	Save starting rotation for x and y (z will always be 0; no fancy movie camera rotations here)
		Vector3 angles = transform.eulerAngles;
		m_CamAngleX = angles.y;
		m_CamAngleY = angles.x;

		m_LightRotationOffset = m_DirectionalLight.transform.rotation;

		m_PuttingScript = GameObject.FindGameObjectWithTag (M_PLAYERTAG).GetComponentInChildren<PuttingScript> ();
		if (!m_PuttingScript)
		{
			print ("Error: Cannot find PuttingScript.");
		}
	}

	private void LateUpdate () 
	{
		//	Only allow camera control when the ball is moving, or the player isn't currently using the putt meter
		if (!m_PuttingScript.isActiveAndEnabled || m_PuttingScript.GetCurrentPuttForce() == 0)
			SetRotation ();
		
		SetPosition ();
	}


	private void SetRotation()
	{
		if (m_Target)
		{
			m_CamAngleX += Input.GetAxis ("Mouse X") * m_XSpeed * m_Distance * Time.deltaTime;

			m_Rotation = Quaternion.Euler(m_CamAngleY, m_CamAngleX, 0);
			transform.rotation = m_Rotation;

			m_DirectionalLight.transform.rotation = m_Rotation * m_LightRotationOffset;

			RotateBall ();
		}
	}


	//	Rotate the ball so that it's forward vector matches the camera but with no rotation on the x-axis
	private void RotateBall()
	{
		Vector3 ballRotation = m_Rotation.eulerAngles;
		m_Target.rotation = Quaternion.Euler(0f, ballRotation.y, ballRotation.z);
	}


	private void SetPosition()
	{
		Vector3 negDistance = new Vector3(0.0f, 0.0f, -m_Distance);
		Vector3 position = m_Rotation * negDistance + m_Target.position;

		transform.position = position;
	}
}