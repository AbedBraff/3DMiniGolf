using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleOverview : MonoBehaviour
{
    public float m_TimeInSecondsToReachOverviewLoc;


    private float m_TranslationDistance;
    private float m_RotationDistance;
    private Transform m_HoleOverviewLoc;
    private Camera m_HoleOverviewCamera;
    private Camera m_MainCamera;


    private void Awake()
    {
        m_HoleOverviewCamera = CameraManager.cameraManager.m_HoleOverviewCamera;
        m_MainCamera = CameraManager.cameraManager.m_MainCamera;
    }


    void Update()
    {
        //  Move position and rotation towards overview loc so they will reach it at the same time
        m_HoleOverviewCamera.transform.position = Vector3.MoveTowards(m_HoleOverviewCamera.transform.position, m_HoleOverviewLoc.position,
            m_TranslationDistance / m_TimeInSecondsToReachOverviewLoc * Time.deltaTime);
        m_HoleOverviewCamera.transform.rotation = Quaternion.RotateTowards(m_HoleOverviewCamera.transform.rotation, m_HoleOverviewLoc.rotation,
            m_RotationDistance / m_TimeInSecondsToReachOverviewLoc * Time.deltaTime);
    }


    public void OverviewLocationSetup()
    {
        //  Move overviewcam to maincam location to begin movement
        m_HoleOverviewCamera.transform.position = m_MainCamera.transform.position;
        m_HoleOverviewCamera.transform.rotation = m_MainCamera.transform.rotation;

        //  Get the hole overview location
        m_HoleOverviewLoc = GameManager.gameManager.m_CurrentCourse.m_CourseHoles[GameManager.gameManager.CurrentPlayer.CurrentHole].m_HoleOverviewLoc;

        //  Figure out the translation and rotation distance between the two transforms
        m_TranslationDistance = (m_HoleOverviewLoc.position - m_HoleOverviewCamera.transform.position).magnitude;
        m_RotationDistance = Quaternion.Angle(m_HoleOverviewLoc.rotation, m_HoleOverviewCamera.transform.rotation);
    }
}
