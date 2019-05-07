using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager m_UIManager;
    public Slider m_Slider;
    public Text m_CurrentHoleText;
    public Text m_CurrentShotsText;
    public Text m_CurrentParText;
    public Color m_TextColor;
    [Range(0f, 1f)] public float m_PanelAlpha;


    private void Awake()
    {
        if(m_UIManager == null)
        {
            DontDestroyOnLoad(gameObject);
            m_UIManager = this;
        }
        else if (m_UIManager != this)
            Destroy(gameObject);

        SetTextColor();
        SetPanelAlpha();
    }


    private void SetTextColor()
    {
        m_CurrentHoleText.color = m_TextColor;
        m_CurrentShotsText.color = m_TextColor;
        m_CurrentParText.color = m_TextColor;
    }


    private void SetPanelAlpha()
    {
        Color color = Color.black;
        color.a = m_PanelAlpha;

        m_CurrentParText.transform.parent.GetComponent<Image>().color = color;
        m_CurrentShotsText.transform.parent.GetComponent<Image>().color = color;
        m_CurrentHoleText.transform.parent.GetComponent<Image>().color = color;
    }
}
