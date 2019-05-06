/*
 * Zachary Mitchell
 * 3DGolfwithNoFriends
 */


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BallMovingScript : MonoBehaviour
{
    public AudioClip m_WallBounceClip;
    public AudioClip m_GrassBounceClip;
    public Vector3 m_GrassParticlesOffset;
    public float m_StoppingSpeed = .05f;
    public float m_PitchRange;
    public float m_AirTimeToResetBall;
    public float m_OOBAnimationDuration;
    public float m_SoundSmoothing = -0.5f;
    public float m_GrassParticleEmissionRate = 50f;
    public float m_MinAirTimeToPlayGroundCollisionAudio = .15f;


    private Animator m_OOBTextAnimator;
    private AudioSource m_MainAudioSource;
    private PuttingScript m_PuttingScript;
    private PlayerManager m_PlayerManagerScript;
    private Rigidbody m_BallRigidBody;
    private Quaternion m_OriginalParticleRotation;
    private Vector3 m_BallPreviousPos;
    private Vector3 m_PreviousFrameVelocity;
    private ParticleSystem m_GrassParticles;
    private float m_OriginalPitch;
    private float m_TimeInAir;
    private float m_OOBResetTimer;
    private bool m_HasOOBAnimStarted;
    private bool m_IsInHole;


    private const string M_WALLTAG = "Wall";
    private const string M_GROUNDTAG = "Ground";
    private const string M_HOLETAG = "Hole";
    private const string M_OOBTEXTTRIGGER = "IsOutOfBounds";
    private const string M_COURSEHOLEPREFIX = "CourseHole";
    private ColliderStack m_CurrentCollidersStack;


    public void Setup()
    {
        //  Find and cache needed scripts and components
        try
        {
            m_MainAudioSource = GetComponent<AudioSource>();
            m_PuttingScript = GetComponent<PuttingScript>();
            m_BallRigidBody = GetComponent<Rigidbody>();
            m_PlayerManagerScript = this.transform.parent.gameObject.GetComponent<PlayerManager>();
            m_GrassParticles = ParticleManager.m_ParticleManager.m_GrassParticles;
            m_OOBTextAnimator = UIManager.m_UIManager.GetComponentInChildren<Animator>(true);
        }
        catch (Exception e)
        {
            print("Error: " + e.ToString());
        }


        //  Create an instance of our colliderstack
        m_CurrentCollidersStack = new ColliderStack();

        //  Set vars to starting values
        ResetVariables();
        m_OriginalPitch = m_MainAudioSource.pitch;
        m_OriginalParticleRotation = m_GrassParticles.transform.rotation;

        this.enabled = false;
    }


    //  Called each time the ball is putt again - resets all needed variables to starting values
    private void OnEnable()
    {
        m_BallPreviousPos = gameObject.transform.position;  //  Only called with OnEnable as during setup it has no previous position
        ResetVariables();
    }


    //  Reset vars to starting values
    private void ResetVariables()
    {
        m_GrassParticles.gameObject.SetActive(false);
        m_TimeInAir = 0.0f;
        m_HasOOBAnimStarted = false;
        m_OOBResetTimer = 0.0f;
        m_IsInHole = false;
        m_PlayerManagerScript.isInHole = false;
    }


    private void Update()
    {
        //  Perform standard updates either when the ball is on the ground, or is in the air but under the reset time
        if (!m_CurrentCollidersStack.IsEmpty() || (m_CurrentCollidersStack.IsEmpty() && (m_TimeInAir <= m_AirTimeToResetBall)))
        {
            if (m_CurrentCollidersStack.IsEmpty())
                m_TimeInAir += Time.deltaTime;


            //	Update our particle effects for grass behind the ball as it moves
            UpdateGrassParticles();

            //  Update previous frame velocity value for next frame
            m_PreviousFrameVelocity = m_BallRigidBody.velocity;
        }
        /*  The ball has been in the air over the time limit
            *  Start out of bounds animation if not yet triggered this putt
            *  Check ensures the animation is not started multiple times
            */
        else if (m_TimeInAir > m_AirTimeToResetBall)
        {
            if (!m_HasOOBAnimStarted)
            {
                m_OOBTextAnimator.SetTrigger(M_OOBTEXTTRIGGER);
                m_HasOOBAnimStarted = true;
            }
        }


        //  Increment out of bounds reset timer if the animation has started
        if (m_HasOOBAnimStarted)
        {
            m_OOBResetTimer += Time.deltaTime;

            //  Stop and reset the ball when the animation has finished playing
            if (m_OOBResetTimer >= m_OOBAnimationDuration)
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
            StopBall();

            /*  Start out of bounds animation if either:
                *  (1) The ball is not on the ground(fairway) and also not in the whole OR
                *  (2) The ball came to a stop within the ground of the wrong hole#
                */
            if ((!m_CurrentCollidersStack.IsTagInStack(M_GROUNDTAG) && !m_IsInHole) || !IsBallWithinProperHoleNumber())
            {
                //  Check if out of bounds animation has been triggered yet this putt and play if it has not been
                if (!m_HasOOBAnimStarted)
                {
                    m_OOBTextAnimator.SetTrigger(M_OOBTEXTTRIGGER);
                    m_HasOOBAnimStarted = true;
                }
            }
            //  If the ball is in the hole on the valid hole#
            else if (m_IsInHole && IsBallWithinProperHoleNumber())
            {
                //TODO
                //UPDATES FOR THE BALL ENDING MOVEMENT IN THE PROPER HOLE
                m_PlayerManagerScript.currentHole = (m_PlayerManagerScript.currentHole++);
                m_PlayerManagerScript.isInHole = m_IsInHole;
                TransferControlToPutting();
            }
            //  Start the next putt sequence directly otherwise
            else
                TransferControlToPutting();
        }
    }


    private float DetermineAudioVolume()
    {
        //	Update any volume that is based on ball velocity per frame
        return (1f - (Mathf.Exp(m_SoundSmoothing * m_PreviousFrameVelocity.magnitude)));
    }


    private void OnTriggerEnter(Collider _col)
    {
        //  Check if ball has entered hole and set if true
        if (_col.CompareTag(M_HOLETAG))
            m_IsInHole = true;
    }


    private void OnTriggerExit(Collider _col)
    {
        //  Check if ball has left hole and set accordingly
        if (_col.CompareTag(M_HOLETAG))
            m_IsInHole = false;
    }


    private void OnCollisionEnter(Collision _collision)
    {
        //	Play audio for wall bounce if ball hits a wall or the inside of the hole
        if (_collision.gameObject.CompareTag(M_WALLTAG) || (_collision.gameObject.CompareTag(M_HOLETAG) && m_IsInHole))
            PlayWallHitAudio();
        /* Check to see if ball is bouncing off the grass/ground by verifying that both:
            * (1) The object collided with is the ground AND
            * (2) The dot product of the ball's velocity and the the down vector (0, -1, 0) is greater than 0 meaning that the ball's velocity has some downward component
            */
        else if (_collision.gameObject.CompareTag(M_GROUNDTAG) &&
            Vector3.Dot((-_collision.transform.up), m_PreviousFrameVelocity) > 0f)
        {
            //  Only play if the ball has been in the air over the value of the cutoff; restricts audio from playing when moving between tiles
            if (m_TimeInAir > m_MinAirTimeToPlayGroundCollisionAudio)
                PlayGrassHitAudio();
        }

        //  Reset time in the air value since we have now collided with at least one object
        m_TimeInAir = 0.0f;


        //  Add the collider to the stack of current colliders the ball is touching
        m_CurrentCollidersStack.Push(_collision.collider);
    }


    private void OnCollisionExit(Collision _collision)
    {
        //  Remove the collider from the list of current colliders the ball is touching
        for (int i = 0; i < m_CurrentCollidersStack.GetNumObjects(); ++i)
        {
            if (m_CurrentCollidersStack.GetObjectAt(i) == _collision.collider)
            {
                m_CurrentCollidersStack.RemoveAt(i);
                break;
            }
        }
    }


    //  Check if the ball is in the proper hole# area
    //  For example, if the player is currently on hole 4, but somehow the ball ends up on the grass of hole 5, use this to verify and reset the ball accordingly
    private bool IsBallWithinProperHoleNumber()
    {
        //  Cycle through all the colliders currently touching the ball
        for (int i = 0; i < m_CurrentCollidersStack.GetNumObjects(); ++i)
        {
            //  Go up a level until you reach a parent whose name starts with 'CourseHole' signifying you have found the parent GameObject for that hole#
            GameObject temp = m_CurrentCollidersStack.GetObjectAt(i).gameObject;
            while (!temp.name.StartsWith(M_COURSEHOLEPREFIX))
                temp = temp.transform.parent.gameObject;


            //  Check if the hole the ball is within matches the current hole of the player
            if (temp.name == m_PlayerManagerScript.GetCurrentHoleAsString())
                return true;
        }

        return false;
    }


    //	Hitting side walls audio
    private void PlayWallHitAudio()
    {
        m_MainAudioSource.clip = m_WallBounceClip;
        m_MainAudioSource.volume = DetermineAudioVolume();
        m_MainAudioSource.Play();
    }


    //	Hitting/bouncing on grass audio
    private void PlayGrassHitAudio()
    {
        m_MainAudioSource.clip = m_GrassBounceClip;
        m_MainAudioSource.volume = DetermineAudioVolume();
        m_MainAudioSource.Play();
    }


    private void ResetBallToPreviousPos()
    {
        m_PreviousFrameVelocity = Vector3.zero;
        gameObject.transform.position = m_BallPreviousPos;
        TransferControlToPutting();
    }


    private void StopBall()
    {
        //	Set velocity to zero and freeze/unfreeze rotation so rotational inertia doesn't continue to move the ball
        m_BallRigidBody.velocity = Vector3.zero;
        m_BallRigidBody.freezeRotation = true;
        m_BallRigidBody.freezeRotation = false;
    }


    //	Update grass particle effect
    private void UpdateGrassParticles()
    {
        //  Only update and play grass particle effect if ball is on the grass
        if (m_CurrentCollidersStack.IsTagInStack(M_GROUNDTAG))
        {
            m_GrassParticles.gameObject.SetActive(true);
            m_GrassParticles.transform.position = m_BallRigidBody.transform.position + m_GrassParticlesOffset;
            m_GrassParticles.Play();

            //  Only update rotation when moving as passing zero vector into LookRotation will cause an error
            if (m_BallRigidBody.velocity != Vector3.zero)
                m_GrassParticles.transform.rotation = Quaternion.LookRotation(m_BallRigidBody.velocity, Vector3.up)
                                                    * m_OriginalParticleRotation;


            //	Emission rate over time set to sqr rt formula (using ^.5 instead of sqrt) with (m_stoppingSpeed, 0) as initial point
            var tempEmission = m_GrassParticles.emission;
            tempEmission.rateOverTime = (Mathf.Pow((m_BallRigidBody.velocity.magnitude - m_StoppingSpeed),
                0.5f)) * m_GrassParticleEmissionRate;
        }
        //  Turn off particles if not on the grass
        else
            m_GrassParticles.gameObject.SetActive(false);
    }


    public void TransferControlToPutting()
    {
        //testing
        print(m_CurrentCollidersStack.ToString());

        m_PuttingScript.enabled = true;
        m_GrassParticles.gameObject.SetActive(false);
        this.enabled = false;
    }
}
