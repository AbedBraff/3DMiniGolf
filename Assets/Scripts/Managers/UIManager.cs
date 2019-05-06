using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager m_UIManager;
    public Slider m_Slider;


    private void Awake()
    {
        if(m_UIManager == null)
        {
            DontDestroyOnLoad(gameObject);
            m_UIManager = this;
        }
        else if (m_UIManager != this)
            Destroy(gameObject);
    }
}
