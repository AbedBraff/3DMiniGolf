/*
 * Zachary Mitchell 
 * 3DGolfwithNoFriends
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GolfCourse : MonoBehaviour
{
    public GolfHole[] m_CourseHoles;
    public string m_CourseName;


    private int m_NumHoles;
    private int m_CoursePar;


    //  Setters & getters
    public int CourseLength { get { return m_NumHoles; } }
    public int CoursePar { get { return m_CoursePar; } }


    private void Awake()
    {
        m_NumHoles = m_CourseHoles.Length;

        m_CoursePar = 0;
        for(int i = 0; i < m_NumHoles; ++i)
        {
            m_CoursePar += m_CourseHoles[i].m_ParValue;
        }
    }
}
