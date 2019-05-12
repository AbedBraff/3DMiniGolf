using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagpoleMovement : MonoBehaviour
{
    public float m_FlagMovementBounds;
    public float m_HeightToRise;
    public float m_Speed;
    public Transform m_Flag;


    private string m_BallTag = "Ball";
    private int m_NumBallsWithinBounds;
    private int m_RaiseOrLower;             //  1 for raise, -1 for lower
    private float m_RaisedYPos;
    private float m_OriginalYPos;
    private float m_GoalYPos;


    private void Awake()
    {
        GetComponent<SphereCollider>().radius = m_FlagMovementBounds;
        m_OriginalYPos = m_Flag.transform.position.y;
        m_RaisedYPos = m_OriginalYPos + m_HeightToRise;
        m_NumBallsWithinBounds = 0;
        m_RaiseOrLower = 0;
    }


    private void Update()
    {
        //  Only update if the flag needs to be raised and isn't at the top pos OR
        //  needs to be lowered and isn't at the bottom pos
        if((m_RaiseOrLower == 1 && m_Flag.transform.position.y < m_GoalYPos) ||
            (m_RaiseOrLower == -1 && m_Flag.transform.position.y > m_GoalYPos))
        {
            //  Move the flag object
            Vector3 temp = m_Flag.transform.position;
            float amountToMove;
            amountToMove = (m_Speed * m_RaiseOrLower * Time.deltaTime);
            temp.y += amountToMove;
            m_Flag.transform.position = temp;


            //  Subtract movement from the trigger object so that it doesn't move with the flag and cause stuttering when the ball is near the edge of its radius
            temp = gameObject.transform.position;
            temp.y -= amountToMove;
            gameObject.transform.position = temp;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        //  Check if the object entering the trigger is a ball
        if (other.CompareTag(m_BallTag))
        {
            ++m_NumBallsWithinBounds;
            m_RaiseOrLower = 1;
            m_GoalYPos = m_RaisedYPos;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(m_BallTag))
        {
            --m_NumBallsWithinBounds;

            if (m_NumBallsWithinBounds == 0)
            {
                m_RaiseOrLower = -1;
                m_GoalYPos = m_OriginalYPos;
            }
        }
    }
}
