using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager m_ParticleManager;
    public ParticleSystem m_GrassParticles;


    private void Awake()
    {
        if (m_ParticleManager == null)
        {
            DontDestroyOnLoad(gameObject);
            m_ParticleManager = this;
        }
        else if (m_ParticleManager != this)
            Destroy(gameObject);
    }
}
