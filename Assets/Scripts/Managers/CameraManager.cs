using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager m_CameraManager;
    public Camera m_MainCamera;
    public Camera m_HoleOverviewCamera;
    public float m_TimeInSecondsToReachOverviewLoc;


    private float m_TranslationDistance;
    private float m_RotationDistance;
    private Transform m_HoleOverviewLoc;
    private Camera m_ActiveCamera;


    private void Awake()
    {
        if (m_CameraManager == null)
        {
            DontDestroyOnLoad(gameObject);
            m_CameraManager = this;
        }
        else if (m_CameraManager != this)
            Destroy(gameObject);

        m_ActiveCamera = m_MainCamera;
    }


    private void Update()
    {
        print(GameManager.GameStatesClass.m_CurrentGameState);

        //  If user presses camera overview button, hasn't started putting, and Putting is the current gamestate
        //  Meaning the player is in the putting stage of the game loop but is not actively powering up a shot
        if (Input.GetKeyDown(KeyCode.C) && !GameManager.m_GameManager.CurrentPlayer.PuttingScript.StartedPutting
            && GameManager.GameStatesClass.m_CurrentGameState == GameManager.GameStatesClass.GameStates.Putting)
        {
            GameManager.GameStatesClass.m_CurrentGameState = GameManager.GameStatesClass.GameStates.HoleOverview;
            MoveCameraToOverviewLoc();
        }
        //  Camera putting pressed while in overview mode
        //  Immediately switch back to main cam and original position as well as putting gamestate
        else if (Input.GetKeyDown(KeyCode.C) && GameManager.GameStatesClass.m_CurrentGameState == GameManager.GameStatesClass.GameStates.HoleOverview)
        {
            GameManager.GameStatesClass.m_CurrentGameState = GameManager.GameStatesClass.GameStates.Putting;
            ChangeCameras();
        }


        if (GameManager.GameStatesClass.m_CurrentGameState == GameManager.GameStatesClass.GameStates.HoleOverview)
        {
            //  Move position and rotation towards overview loc so they will reach it at the same time
            m_ActiveCamera.transform.position = Vector3.MoveTowards(m_ActiveCamera.transform.position, m_HoleOverviewLoc.position,
                m_TranslationDistance / m_TimeInSecondsToReachOverviewLoc * Time.deltaTime);
            m_ActiveCamera.transform.rotation = Quaternion.RotateTowards(m_ActiveCamera.transform.rotation, m_HoleOverviewLoc.rotation,
                m_RotationDistance / m_TimeInSecondsToReachOverviewLoc * Time.deltaTime);
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


    private void MoveCameraToOverviewLoc()
    {
        //  Move overviewcam to maincam location to begin movement
        m_HoleOverviewCamera.transform.position = m_MainCamera.transform.position;
        m_HoleOverviewCamera.transform.rotation = m_MainCamera.transform.rotation;

        //  Get the hole overview location
        m_HoleOverviewLoc = GameManager.m_GameManager.m_CurrentCourse.m_CourseHoles[GameManager.m_GameManager.CurrentPlayer.CurrentHole].m_HoleOverviewLoc;

        //  Figure out the translation and rotation distance between the two transforms
        m_TranslationDistance = (m_HoleOverviewLoc.position - m_ActiveCamera.transform.position).magnitude;
        m_RotationDistance = Quaternion.Angle(m_HoleOverviewLoc.rotation, m_ActiveCamera.transform.rotation);

        //  Swap cameras
        ChangeCameras();
    }
}
