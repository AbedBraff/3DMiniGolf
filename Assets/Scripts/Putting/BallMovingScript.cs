/*
 * Zachary Mitchell - 10/15/18 - 3dMinigolfwithNoFriends
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallMovingScript : MonoBehaviour {


	public float m_StoppingSpeed = .05f;
	public AudioClip m_WallBounceClip;
	public AudioClip m_GrassBounceClip;
	public AudioSource m_MainAudioSource;
	public float m_PitchRange = .2f;
	public ParticleSystem m_GrassParticles;
	public Vector3 m_GrassParticlesOffset;
    public float m_AirTimeToResetBall;
    public float m_OOBResetTime;
    public Animator m_OOBTextAnimator;


	private PuttingScript m_PuttingScript;
	private Rigidbody m_BallRigidBody;
	private float m_OriginalPitch;
	private Quaternion m_OriginalParticleRotation;
	private float m_AudioVolume;
    private Vector3 m_PreviousFrameVelocity;
    private bool m_IsInAir;
    private float m_TimeInAir;
    private Vector3 m_BallPreviousPos;
    private float m_OOBResetTimer = 0f;
    private bool m_HasOOBAnimStarted;
    private bool m_IsInHole;


	private const string M_WALLTAG = "Wall";
	private const string M_GROUNDTAG = "Ground";
    private const string M_HOLETAG = "Hole";
    private const string M_OOBTEXTTRIGGER = "IsOutOfBounds";
	private const float M_SOUNDSMOOTHING = -0.5f;
	private const float M_GRASSEMISSION = 50f;
    private const float M_AIRTIMECUTOFFVALUE = .15f;


    //private Stack<Collider> m_CurrentCollidersStack = new Stack<Collider>();
    private ColliderStack m_CurrentCollidersStack = new ColliderStack();


	private void Awake()
	{
		m_PuttingScript = GetComponent<PuttingScript> ();
		m_BallRigidBody = GetComponent<Rigidbody> ();

		m_OriginalPitch = m_MainAudioSource.pitch;
		m_OriginalParticleRotation = m_GrassParticles.transform.rotation;

		m_GrassParticles.gameObject.SetActive (false);
        m_IsInAir = false;
        m_HasOOBAnimStarted = false;
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
            if(!m_HasOOBAnimStarted)
            {
                m_OOBTextAnimator.SetTrigger(M_OOBTEXTTRIGGER);
                m_HasOOBAnimStarted = true;
            }
        }


        if(m_HasOOBAnimStarted)
        {
            m_OOBResetTimer += Time.deltaTime;

            if (m_OOBResetTimer >= m_OOBResetTime)
            {
                StopBall();
                ResetBallToPreviousPos();
            }
        }
    }


	private void FixedUpdate()
	{
        //	Stop ball and transfer control if the velocity < m_StoppingSpeed
        if (m_BallRigidBody.velocity.magnitude <= m_StoppingSpeed)
		{
			StopBall ();

            //  Search through the current collider stack and see if the ball is touching part of the valid current hole of the course
            bool isTouchingGround = m_CurrentCollidersStack.IsTagInStack(M_GROUNDTAG);


            //  If the ball is in the hole
            if (m_IsInHole)
            {
                m_GrassParticles.gameObject.SetActive(false);
                Debug.Log("Ball has entered the hole");
            }
            //  If the ball is not touching the ground
            else if (!m_CurrentCollidersStack.IsTagInStack(M_GROUNDTAG))
            {
                //  Check if out of bounds animation has been triggered yet this putt and play if it has not been
                if (!m_HasOOBAnimStarted)
                {
                    m_OOBTextAnimator.SetTrigger(M_OOBTEXTTRIGGER);
                    m_HasOOBAnimStarted = true;
                }
            }
            //  If the ball is touching a valid part of the current hole, we can go ahead and start the next putt
            else
			    TransferControl ();
		}
	}


	private void OnEnable()
	{
        m_OOBResetTimer = 0f;
        m_TimeInAir = 0;
        m_BallPreviousPos = gameObject.transform.position;
        m_HasOOBAnimStarted = false;

        m_GrassParticles.gameObject.SetActive (true);
		m_GrassParticles.Play ();
	}


    private void OnTriggerEnter(Collider _col)
    {
        //  Check if ball has entered hole and set if true
        if (_col.CompareTag(M_HOLETAG))
        {
            m_IsInHole = true;
        }
    }


    private void OnTriggerExit(Collider _col)
    {
        //  Check if ball has left hole and set if true
        if(_col.CompareTag(M_HOLETAG))
        {
            m_IsInHole = false;
        }
    }


    private void OnCollisionEnter(Collision collision)
	{
		//	Play audio for wall bounce if ball hits a wall or the inside of the hole
		if (collision.gameObject.CompareTag (M_WALLTAG) ||
            (collision.gameObject.CompareTag(M_HOLETAG) && m_IsInHole))
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
		m_MainAudioSource.clip = m_WallBounceClip;
		m_MainAudioSource.volume = m_AudioVolume;
		m_MainAudioSource.Play ();
	}


	//	Hitting/bouncing on grass audio
	private void PlayGrassHitAudio()
	{
		m_MainAudioSource.clip = m_GrassBounceClip;
		m_MainAudioSource.volume = m_AudioVolume;
		m_MainAudioSource.Play ();
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
        //  Only update and play if ball is rolling on the grass
        if (m_CurrentCollidersStack.IsTagInStack(M_GROUNDTAG))
        {
            m_GrassParticles.transform.position = m_BallRigidBody.transform.position + m_GrassParticlesOffset;

            if (m_BallRigidBody.velocity != Vector3.zero)
                m_GrassParticles.transform.rotation = Quaternion.LookRotation(m_BallRigidBody.velocity, Vector3.up)
                                                    * m_OriginalParticleRotation;

            //	Emission rate over time set to sqr rt formula with (m_stoppingSpeed, 0) as initial point
            var tempEmission = m_GrassParticles.emission;
            tempEmission.rateOverTime = (Mathf.Pow((m_BallRigidBody.velocity.magnitude - m_StoppingSpeed),
                0.5f)) * M_GRASSEMISSION;
        }
	}


	public void TransferControl()
	{
		m_PuttingScript.enabled = true;
		m_GrassParticles.gameObject.SetActive (false);
		this.enabled = false;
	}
}
