/*
 * Zachary Mitchell
 * 3DGolfwithNoFriends
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //  Setters & getters
    public GameObject Instance{ get { return m_Instance; } set { m_Instance = value; } }
    public int PlayerNumber { set { m_PlayerNumber = value; } }
    public int CurrentHole { get { return m_CurrentHole; } set { m_CurrentHole = value; } }
    public string CurrentHoleAsString { get { return "CourseHole" + (m_CurrentHole + 1); } }
    public bool IsInHole{ get { return m_IsInHole; } set { m_IsInHole = value; } }
    public int CurrentHoleShots{ get { return m_CurrentHoleShots; } set { m_CurrentHoleShots = value; } }
    public int TotalCourseShots{ get { return m_TotalCourseShots; } set { m_TotalCourseShots = value; } }
    public PuttingScript PuttingScript { get { return m_PuttingScript; } }


    private GameObject m_Instance;
    private PuttingScript m_PuttingScript;
    private BallMovingScript m_BallMovingScript;
    private int m_PlayerNumber;
    private int m_CurrentHole;
    private bool m_IsInHole;
    private int m_CurrentHoleShots;
    private int m_TotalCourseShots;


    public void Setup()
    {
        m_CurrentHoleShots = 0;
        m_TotalCourseShots = 0;
        m_CurrentHole = 0;
        m_PuttingScript = m_Instance.GetComponentInChildren<PuttingScript>(true);
        m_BallMovingScript = m_Instance.GetComponentInChildren<BallMovingScript>(true);
        m_PuttingScript.Setup();
        m_BallMovingScript.Setup();

        GameManager.GameStatesClass.m_CurrentGameState = GameManager.GameStatesClass.GameStates.Setup;
    }


    public void EnablePutting()
    {
        m_PuttingScript.enabled = true;
    }


    public void DisablePutting()
    {
        m_PuttingScript.enabled = false;
    }
}
