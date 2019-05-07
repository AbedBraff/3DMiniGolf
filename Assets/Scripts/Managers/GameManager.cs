/*
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
    public float m_HoleStartDelay = 3f;
    public float m_HoleEndDelay = 1f;


    private int m_NumPlayers;
    private int m_CurrentPlayer;
    private PlayerManager[] m_Players;
    private GameObject[] m_PlayerObjects;


    //  Public enum class with static public vars for game state changes
    public class GameStatesClass : MonoBehaviour
    {
        public enum GameStates { Menu, Paused, Starting, Playing, Ending, Setup };
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
        UIManager.m_UIManager.m_CurrentHoleText.text = "Hole\n" + (m_Players[m_CurrentPlayer].currentHole + 1) + " of " + m_CurrentCourse.m_CourseHoles.Length;
        //  Set the UI text for the current hole par value
        UIManager.m_UIManager.m_CurrentParText.text = "Par\n" + m_CurrentCourse.m_CourseHoles[m_Players[m_CurrentPlayer].currentHole].m_ParValue;
        //  Reset the UI text for current shots back to 0
        UIManager.m_UIManager.m_CurrentShotsText.text = "Shots:\t0";


        //  Get the rigidbody component of the current player (currently only a 1 player game so currentplayer is always 0)
        Rigidbody rb = m_Players[m_CurrentPlayer].GetComponentInChildren<Rigidbody>();

        //  Bring the ball to a complete stop
        rb.velocity = Vector3.zero;
        rb.freezeRotation = true;
        rb.freezeRotation = false;

        //  Move the ball to the starting position and rotation of the next hole
        rb.MovePosition(m_CurrentCourse.m_CourseHoles[m_Players[m_CurrentPlayer].currentHole].m_StartingLoc.position);

        Vector3 rotation = m_CurrentCourse.m_CourseHoles[m_Players[m_CurrentPlayer].currentHole].m_StartingLoc.eulerAngles;
        rb.MoveRotation(Quaternion.Euler(rotation));
        rotation += m_CameraControl.defaultAngles;
        m_CameraControl.camAngleX = rotation.y;
        m_CameraControl.camAngleY = rotation.x;


        //  Delay
        yield return new WaitForSeconds(m_HoleStartDelay);
    }


    private IEnumerator HolePlaying()
    {
        GameManager.GameStatesClass.m_CurrentGameState = GameStatesClass.GameStates.Playing;
        m_Players[0].EnablePutting();

        //  PLACEHOLDER
        while(!m_Players[m_CurrentPlayer].isInHole)
        {
            yield return null;
        }
    }


    private IEnumerator HoleEnding()
    {
        GameManager.GameStatesClass.m_CurrentGameState = GameStatesClass.GameStates.Ending;
        Debug.Log("Hole Over");

        m_Players[m_CurrentPlayer].currentHole++;
        m_Players[m_CurrentPlayer].isInHole = false;
        m_Players[m_CurrentPlayer].currentHoleShots = 0;

        //  Delay
        yield return new WaitForSeconds(m_HoleEndDelay);
    }


    private bool IsCourseComplete()
    {
        if (m_Players[m_CurrentPlayer].currentHole == m_CurrentCourse.m_CourseHoles.Length)
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
            m_Players[i].instance = m_PlayerObjects[i];
            m_Players[i].playerNumber = i + 1;
            m_Players[i].Setup();
        }
    }

    private void SetCamera()
    {
        m_CameraControl.target = m_Players[0].instance.GetComponentInChildren<Rigidbody>().transform;
        m_CameraControl.enabled = true;
        m_CameraControl.Setup();
    }
}
