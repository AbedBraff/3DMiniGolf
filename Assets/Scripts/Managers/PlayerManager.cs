/*
 * Zachary Mitchell
 * 3DGolfwithNoFriends
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

     public GameObject m_Instance;
    [HideInInspector] public GameObject m_CanvasGameObject;
    [HideInInspector] public int m_PlayerNumber;
    public GameManager m_GameManagerScript;


    private int m_CurrentHole;
    private bool m_IsInHole;
    private PuttingScript m_PuttingScript;
    private BallMovingScript m_BallMovingScript;
    private int m_CurrentHoleShots;


    private const string M_GAMEMANAGERTAG = "GameManager";


    //  Setters and getters
    public void SetCurrentHole(int _value)
    {
        m_CurrentHole = _value;
    }
    public string GetCurrentHoleAsString()
    {
        return "CourseHole" + (m_CurrentHole + 1);
    }
    public int GetCurrentHoleAsInt()
    {
        return m_CurrentHole;
    }
    public void SetIsInHole(bool _value)
    {
        m_IsInHole = _value;
    }
    public bool GetIsInHole()
    {
        return m_IsInHole;
    }
    public int GetCurrentHoleShots()
    {
        return m_CurrentHoleShots;
    }
    public void SetCurrentHoleShots(int _value)
    {
        m_CurrentHoleShots = _value;
        print(m_CurrentHoleShots);
    }


    public void Setup()
    {
        m_GameManagerScript = GameObject.FindGameObjectWithTag(M_GAMEMANAGERTAG).GetComponent<GameManager>();
        m_CurrentHole = 0;
        m_PuttingScript = m_Instance.GetComponentInChildren<PuttingScript>(true);
        m_BallMovingScript = m_Instance.GetComponentInChildren<BallMovingScript>(true);
        m_PuttingScript.Setup();
        m_BallMovingScript.Setup();
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
