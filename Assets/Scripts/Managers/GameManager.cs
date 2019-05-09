﻿/*
 * Zachary Mitchell
 * 3DGolfwithNoFriends
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {


    public static GameManager m_GameManager;


    public GameObject m_PlayerPrefab;
    public CameraControl m_CameraControl;
    public GolfCourse m_CurrentCourse;
    public float m_HoleStartDelay;
    public float m_HoleEndDelay;
    public float m_HoleResultDelay;


    private int m_NumPlayers;
    private int m_CurrentPlayer;
    private PlayerManager[] m_Players;
    private GameObject[] m_PlayerObjects;


    public PlayerManager CurrentPlayer { get { return m_Players[m_CurrentPlayer]; } }


    //  Public enum class with static public vars for game state changes
    public class GameStatesClass : MonoBehaviour
    {
        public enum GameStates { Menu, Paused, Starting, Playing, Ending, Putting, BallMoving, HoleOverview, Setup };
        public static GameStates m_CurrentGameState;
    }


    private void Awake()
    {
        if(m_GameManager == null)
        {
            DontDestroyOnLoad(gameObject);
            m_GameManager = this;
            GameManager.GameStatesClass.m_CurrentGameState = GameStatesClass.GameStates.Menu;
        }
        else if (m_GameManager != this)
            Destroy(gameObject);
    }


    private void Update()
    {
        //  Start game if we are in the "menu" (not yet implemented) and press enter
        if(GameManager.GameStatesClass.m_CurrentGameState == GameStatesClass.GameStates.Menu && Input.GetKeyDown(KeyCode.Return))
        {
            Setup();
        }
    }


    private void Setup () 
	{
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        GameManager.GameStatesClass.m_CurrentGameState = GameStatesClass.GameStates.Setup;
        m_NumPlayers = 1;
        m_CurrentPlayer = 0;
        m_Players = new PlayerManager[m_NumPlayers];
        m_PlayerObjects = new GameObject[m_NumPlayers];

        SpawnPlayers();
        SetCamera();

        StartCoroutine(GameLoop());
	}


    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(HoleStarting());
        yield return StartCoroutine(HolePlaying());
        yield return StartCoroutine(HoleEnding());


        //  Determine if we've reached the end of the course
        if (IsCourseComplete())
        {
            print("Course over");
        }
        //  Move on to the next hole if not
        else
            StartCoroutine(GameLoop());


        yield return null;
    }


    private IEnumerator HoleStarting()
    {
        GameManager.GameStatesClass.m_CurrentGameState = GameStatesClass.GameStates.Starting;


        //  Set the UI text for the current hole number
        UIManager.m_UIManager.m_CurrentHoleText.text = "Hole\n" + (m_Players[m_CurrentPlayer].CurrentHole + 1) + " of " + m_CurrentCourse.m_CourseHoles.Length;
        //  Set the UI text for the current hole par value
        UIManager.m_UIManager.m_CurrentParText.text = "Par\n" + m_CurrentCourse.m_CourseHoles[m_Players[m_CurrentPlayer].CurrentHole].m_ParValue;
        //  Reset the UI text for current shots back to 0
        UIManager.m_UIManager.m_CurrentShotsText.text = "Shots:\t0";


        //  Get the rigidbody component of the current player (currently only a 1 player game so currentplayer is always 0)
        Rigidbody rb = m_Players[m_CurrentPlayer].GetComponentInChildren<Rigidbody>();

        //  Bring the ball to a complete stop
        rb.velocity = Vector3.zero;
        rb.freezeRotation = true;
        rb.freezeRotation = false;

        //  Move the ball to the starting position and rotation of the next hole
        rb.MovePosition(m_CurrentCourse.m_CourseHoles[m_Players[m_CurrentPlayer].CurrentHole].m_StartingLoc.position);

        Vector3 rotation = m_CurrentCourse.m_CourseHoles[m_Players[m_CurrentPlayer].CurrentHole].m_StartingLoc.eulerAngles;
        rb.MoveRotation(Quaternion.Euler(rotation));
        rotation += m_CameraControl.DefaultAngles;
        m_CameraControl.CamAngleX = rotation.y;
        m_CameraControl.CamAngleY = rotation.x;


        //  Delay
        yield return new WaitForSeconds(m_HoleStartDelay);
    }


    private IEnumerator HolePlaying()
    {
        GameManager.GameStatesClass.m_CurrentGameState = GameStatesClass.GameStates.Playing;
        m_Players[m_CurrentPlayer].EnablePutting();

        //  PLACEHOLDER
        while(!m_Players[m_CurrentPlayer].IsInHole)
        {
            yield return null;
        }
    }


    private IEnumerator HoleEnding()
    {
        GameManager.GameStatesClass.m_CurrentGameState = GameStatesClass.GameStates.Ending;
        Debug.Log("Hole Over");


        //  Set text for hole result text
        UIManager.m_UIManager.m_HoleResultText.text = GetHoleResultText();

        //  Hole result delay so that the player can see their result
        yield return new WaitForSeconds(m_HoleResultDelay);

        //  Reset hole result text
        UIManager.m_UIManager.m_HoleResultText.text = "";


        m_Players[m_CurrentPlayer].CurrentHole++;
        m_Players[m_CurrentPlayer].IsInHole = false;
        m_Players[m_CurrentPlayer].CurrentHoleShots = 0;
        //  PUTTING MUST BE DISABLED HERE
        //  Because BallMoving naturally progresses into Putting once the ball has stopped, we must disable it between holes to make sure values & game states are properly reset
        m_Players[m_CurrentPlayer].DisablePutting();


        //  Delay between end of one hole and start of the next
        yield return new WaitForSeconds(m_HoleEndDelay);
    }


    //  Return the result of a hole; ie birdie, hole in one, bogey, etc
    private string GetHoleResultText()
    {
        string result = "";

        //  Check if player got a hole in one
        if (m_Players[m_CurrentPlayer].CurrentHoleShots == 1)
            return "Hole in one!";


        //  Determine result by subtracting the player's shots for that hole from the par value
        int relativeScore = (m_Players[m_CurrentPlayer].CurrentHoleShots) -
            (m_CurrentCourse.m_CourseHoles[m_Players[m_CurrentPlayer].CurrentHole].m_ParValue);

        if (relativeScore <= -3)
            result = "Albatross";
        else if (relativeScore == -2)
            result = "Eagle";
        else if (relativeScore == -1)
            result = "Birdie";
        else if (relativeScore == 0)
            result = "Par";
        else if (relativeScore == 1)
            result = "Bogey";
        else if (relativeScore == 2)
            result = "Double Bogey";
        else if (relativeScore == 3)
            result = "Triple Bogey";
        else
            result = "+" + relativeScore;
 

        return result;
    }


    private bool IsCourseComplete()
    {
        if (m_Players[m_CurrentPlayer].CurrentHole == m_CurrentCourse.m_CourseHoles.Length)
        {
            return true;
        }

        return false;
    }


    private void SpawnPlayers()
    {
        for (int i = 0; i < m_NumPlayers; ++i)
        {
            m_PlayerObjects[i] = Instantiate(m_PlayerPrefab) as GameObject;
            m_Players[i] = m_PlayerObjects[i].GetComponent<PlayerManager>();
            m_Players[i].Instance = m_PlayerObjects[i];
            m_Players[i].PlayerNumber = i + 1;
            m_Players[i].Setup();
        }
    }

    private void SetCamera()
    {
        m_CameraControl.Target = m_Players[0].Instance.GetComponentInChildren<Rigidbody>().transform;
        m_CameraControl.enabled = true;
        m_CameraControl.Setup();
    }
}
