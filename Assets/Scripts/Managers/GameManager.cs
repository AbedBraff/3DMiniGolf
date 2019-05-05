/*
 * Zachary Mitchell
 * 3DGolfwithNoFriends
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {


    public GameObject m_PlayerPrefab;
    public CameraControl m_CameraControl;
    public float m_GameStartDelay = 1f;
    public GameObject m_CanvasGameObject;
    public GolfCourse m_CurrentCourse;
    public PlayerManager[] m_Players;


    private string m_CurrentGameState;
    private int m_NumPlayers;
    private WaitForSeconds m_GameStartWait;
    private GameObject[] m_PlayerObjects;

    private string[] m_GameStates = { "Menu", "Paused", "Playing", "Setup", "Ending" };
    public string GameMenuString()
    {
        return m_GameStates[0];
    }
    public string GamePausedString()
    {
        return m_GameStates[1];
    }
    public string GamePlayingString()
    {
        return m_GameStates[2];
    }
    public string GameSetupString()
    {
        return m_GameStates[3];
    }
    public string GameEndingString()
    {
        return m_GameStates[4];
    }
    public string GetGameState()
    {
        return m_CurrentGameState;
    }
    public void SetGameStateToSetup()
    {
        m_CurrentGameState = m_GameStates[3];
    }
    public void SetGameStateToPlaying()
    {
        m_CurrentGameState = m_GameStates[2];
    }


	void Start () 
	{
		//	Placeholder for now to make testing easier
		Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        m_NumPlayers = 1;
        m_Players = new PlayerManager[m_NumPlayers];
        m_PlayerObjects = new GameObject[m_NumPlayers];

        m_GameStartWait = new WaitForSeconds(m_GameStartDelay);

        SetGameStateToSetup();
        SpawnPlayers();
        SetCamera();

        StartCoroutine(GameLoop());
	}


    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(Starting());
        Debug.Log("GameStarting finished");
        yield return StartCoroutine(Playing());
        //yield return StartCoroutine(Ending());

        Debug.Log("Hole Over");

        if (m_Players[0].GetCurrentHoleAsInt() == m_CurrentCourse.m_CourseHoles.Length)
            Debug.Log("Game Over");
        else
            StartCoroutine(GameLoop());

        yield return null;
    }


    private IEnumerator Starting()
    {
        yield return m_GameStartWait;
    }


    private IEnumerator Playing()
    {
        SetGameStateToPlaying();
        m_Players[0].SetIsInHole(false);
        m_Players[0].gameObject.GetComponentInChildren<Rigidbody>().MovePosition(m_CurrentCourse.m_CourseHoles[m_Players[0].GetCurrentHoleAsInt()].m_StartingLoc.position);
        m_Players[0].gameObject.GetComponentInChildren<Rigidbody>().velocity = Vector3.zero;
        m_Players[0].gameObject.GetComponentInChildren<Rigidbody>().freezeRotation = true;
        m_Players[0].gameObject.GetComponentInChildren<Rigidbody>().freezeRotation = false;
        m_Players[0].EnablePutting();
        m_CameraControl.enabled = true;

        //  PLACEHOLDER
        while(!m_Players[0].GetIsInHole())
        {
            yield return null;
        }
    }


    //TODO
    private IEnumerator Ending()
    {
        yield return null;
    }


    private void SpawnPlayers()
    {
        Debug.Log("SpawnPlayers called");

        //  Placeholder for starting a game
        for (int i = 0; i < m_NumPlayers; ++i)
        {
            m_PlayerObjects[i] = Instantiate(m_PlayerPrefab) as GameObject;
            m_Players[i] = m_PlayerObjects[i].GetComponent<PlayerManager>();
            m_Players[i].m_Instance = m_PlayerObjects[i];
            m_Players[i].m_PlayerNumber = i + 1;
            m_Players[i].m_CanvasGameObject = m_CanvasGameObject;
            m_Players[i].Setup();
        }
    }

    private void SetCamera()
    {
        m_CameraControl.SetTarget(m_Players[0].m_Instance.GetComponentInChildren<Rigidbody>().transform);
        m_CameraControl.Setup();
    }
}
