/*
 * Zachary Mitchell - 10/15/18 - 3dMinigolfwithNoFriends
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovingScript : MonoBehaviour {


	public float m_StoppingSpeed = .05f;
	public AudioClip m_WallBounceClip;
	public AudioClip m_GrassBounceClip;
	public AudioSource m_SecondaryAudioSource;
	public AudioSource m_RollingAudioSource;
	public float m_PitchRange = .2f;
	public ParticleSystem m_GrassParticles;
	public Vector3 m_GrassParticlesOffset;
    public float m_AirTimeToResetBall;


	private PuttingScript m_PuttingScript;
	private Rigidbody m_BallRigidBody;
	private float m_OriginalPitch;
	private Quaternion m_OriginalParticleRotation;
	private float m_AudioVolume;
    private Vector3 m_PreviousFrameVelocity;
    private bool m_IsInAir;
    private float m_TimeInAir;
    private Vector3 m_BallPreviousPos;


	private const string M_WALLTAG = "Wall";
	private const string M_GROUNDTAG = "Ground";
	private const float M_SOUNDSMOOTHING = -0.5f;
	private const float M_GRASSEMISSION = 40f;
    private const float M_AIRTIMECUTOFFVALUE = .15f;


    private static Stack<Collider> m_CurrentCollidersStack = new Stack<Collider>();


	private void Awake()
	{
		m_PuttingScript = GetComponent<PuttingScript> ();
		m_BallRigidBody = GetComponent<Rigidbody> ();

		m_OriginalPitch = m_SecondaryAudioSource.pitch;
		m_OriginalParticleRotation = m_GrassParticles.transform.rotation;

		m_GrassParticles.gameObject.SetActive (false);
        m_IsInAir = false;
	}


	private void Update()
	{
        //  When the ball isn't in the air, or is in the air but under the reset time, perform standard updates
        if(!m_IsInAir || (m_IsInAir && (m_TimeInAir <= m_AirTimeToResetBall)))
        {
            if(m_IsInAir)
            {
                m_TimeInAir += Time.deltaTime;
            }


            //	Update any volume that is based on ball velocity per frame
            m_AudioVolume = (1f - (Mathf.Exp(M_SOUNDSMOOTHING * m_PreviousFrameVelocity.magnitude)));

            //	Update our particle effects for grass behind the ball as it moves
            UpdateGrassParticles();

            //  Update previous frame velocity value for next frame
            m_PreviousFrameVelocity = m_BallRigidBody.velocity;
        }
        //  The ball has been in the air over the time limit; reset to previous position
        else if (m_TimeInAir > m_AirTimeToResetBall)
        {
            StopBall();
            ResetBallToPreviousPos();
        }
    }


	private void FixedUpdate()
	{
        //	Stop ball and transfer control if the velocity < m_StoppingSpeed
        if (m_BallRigidBody.velocity.magnitude <= m_StoppingSpeed)
		{
			StopBall ();

            bool isTouchingGround = false;

            for(int i = 0; i < m_CurrentCollidersStack.GetNumObjects(); ++i)
            {
                if(m_CurrentCollidersStack.GetObjectAt(i).CompareTag(M_GROUNDTAG))
                {
                    isTouchingGround = true;
                    break;
                }
            }

            if (!isTouchingGround)
                ResetBallToPreviousPos();

			TransferControl ();
		}
	}


	private void OnEnable()
	{
        m_TimeInAir = 0;
        m_BallPreviousPos = gameObject.transform.position;

        m_GrassParticles.gameObject.SetActive (true);
		m_GrassParticles.Play ();
	}
		

	private void OnCollisionEnter(Collision collision)
	{
		//	Play audio for wall bounce if ball hits a wall with volume based on velocity
		if (collision.gameObject.CompareTag (M_WALLTAG))
		{
			PlayWallHitAudio ();
		}
        /* Checks to see if ball is bouncing off the grass/ground:
		 * 1) Continue if the object collided with is the ground AND
		 * 2) if the dot product of the ball's velocity and the the down vector (0, -1, 0) is greater than 0 meaning that the ball's velocity has some level of downward component
		 */
        else if (collision.gameObject.CompareTag(M_GROUNDTAG) && 
            Vector3.Dot((collision.transform.up * -1), m_PreviousFrameVelocity) > 0f)
        {
            //  Only play if the ball has been in the air over the value of the cutoff; restricts audio from playing when going over bumps between tiles
            if (m_TimeInAir > M_AIRTIMECUTOFFVALUE)
                PlayGrassHitAudio();
        }

        m_IsInAir = false;

        //  Add the collider to the list of current colliders the ball is touching
        m_CurrentCollidersStack.AddAt(collision.collider, m_CurrentCollidersStack.GetNumObjects());
	}


	private void OnCollisionStay(Collision collision)
	{
		//	Play audio for ball rolling on grass if it remains in contact with grass
		if (collision.gameObject.CompareTag (M_GROUNDTAG))
			PlayBallRollingAudio ();

        m_IsInAir = false;
	}


    private void OnCollisionExit(Collision collision)
    {
        //  Remove the collider from the list of current colliders the ball is touching
        for(int i = 0; i < m_CurrentCollidersStack.GetNumObjects(); ++i)
        {
            if(m_CurrentCollidersStack.GetObjectAt(i) == collision.collider)
            {
                m_CurrentCollidersStack.RemoveAt(i);
                break;
            }
        }

        //  Check if there are no current colliders (the ball is in the air) and if true, start the air timer
        if(m_CurrentCollidersStack.IsEmpty())
        {
            m_IsInAir = true;
            m_TimeInAir = 0f;
        }
    }


    //	Hitting side walls audio
    private void PlayWallHitAudio()
	{
		m_SecondaryAudioSource.clip = m_WallBounceClip;
		m_SecondaryAudioSource.volume = m_AudioVolume;
		m_SecondaryAudioSource.Play ();
	}


	//	Hitting/bouncing on grass audio
	private void PlayGrassHitAudio()
	{
		m_SecondaryAudioSource.clip = m_GrassBounceClip;
		m_SecondaryAudioSource.volume = m_AudioVolume;
		m_SecondaryAudioSource.Play ();
	}


	//	Rolling on grass audio
	private void PlayBallRollingAudio()
	{
		m_RollingAudioSource.volume = m_AudioVolume;
		m_RollingAudioSource.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
		m_RollingAudioSource.Play ();
	}


    private void ResetBallToPreviousPos()
    {
        m_PreviousFrameVelocity = Vector3.zero;
        gameObject.transform.position = m_BallPreviousPos;
        TransferControl();
    }


	private void StopBall()
	{
        //	Set velocity to zero and freeze/unfreeze rotation so rotational inertia doesn't continue to move the ball
		m_BallRigidBody.velocity = Vector3.zero;
		m_BallRigidBody.freezeRotation = true;
		m_BallRigidBody.freezeRotation = false;
	}


	//	Update position, rotation, and emission rate over time for kicked up grass particles by the ball moving
	private void UpdateGrassParticles()
	{
		m_GrassParticles.transform.position = m_BallRigidBody.transform.position + m_GrassParticlesOffset;

		m_GrassParticles.transform.rotation = Quaternion.LookRotation(m_BallRigidBody.velocity, Vector3.up)
												* m_OriginalParticleRotation;

		//	Emission rate over time set to sqr rt formula with (m_stoppingSpeed, 0) as initial point
		var tempEmission = m_GrassParticles.emission;
		tempEmission.rateOverTime = (Mathf.Pow ((m_BallRigidBody.velocity.magnitude - m_StoppingSpeed), 
			0.5f)) * M_GRASSEMISSION;
	}


	public void TransferControl()
	{
		m_PuttingScript.enabled = true;
		m_GrassParticles.gameObject.SetActive (false);
		this.enabled = false;
	}
}
