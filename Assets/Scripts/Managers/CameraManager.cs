using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager cameraManager;


    public Camera m_MainCamera;
    public Camera m_HoleOverviewCamera;


    private Camera m_ActiveCamera;
    private HoleOverview m_HoleOverviewScript;


    private void Awake()
    {
        if (cameraManager == null)
        {
            DontDestroyOnLoad(gameObject);
            cameraManager = this;
        }
        else if (cameraManager != this)
            Destroy(gameObject);

        m_ActiveCamera = m_MainCamera;
        try
        {
            m_HoleOverviewScript = m_HoleOverviewCamera.gameObject.GetComponent<HoleOverview>();
        }
        catch(Exception e)
        {
            print("Error: " + e);
        }
    }


    private void Update()
    {
        /*  If user presses camera overview button, hasn't started putting, Putting is the current playerstate
         *  , and playing is the current gamestate
         *   Meaning the player is in the putting stage of the game loop but is not actively powering up a shot
         */
        if (Input.GetKeyDown(KeyCode.C) && !GameManager.gameManager.CurrentPlayer.PuttingScript.StartedPutting
            && GameManager.gameManager.CurrentPlayer.playerState == PlayerState.PlayerStates.Putting
            && GameManager.gameState == GameState.GameStates.Playing)
        {
            GameManager.gameManager.CurrentPlayer.playerState = PlayerState.PlayerStates.HoleOverview;
            ChangeCameras();
            m_HoleOverviewScript.OverviewLocationSetup();
        }
        //  Camera putting pressed while in overview mode
        //  Immediately switch back to main cam and original position as well as putting gamestate
        else if (Input.GetKeyDown(KeyCode.C) && GameManager.gameManager.CurrentPlayer.playerState == PlayerState.PlayerStates.HoleOverview)
        {
            GameManager.gameManager.CurrentPlayer.playerState = PlayerState.PlayerStates.Putting;
            ChangeCameras();
        }
    }


    public void ChangeCameras()
    {
        m_ActiveCamera.gameObject.SetActive(false);

        if (m_ActiveCamera == m_MainCamera)
            m_ActiveCamera = m_HoleOverviewCamera;
        else
            m_ActiveCamera = m_MainCamera;

        m_ActiveCamera.gameObject.SetActive(true);
    }
}
