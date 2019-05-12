using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager particleManager;


    public ParticleSystem m_GrassParticles;


    private void Awake()
    {
        if (particleManager == null)
        {
            DontDestroyOnLoad(gameObject);
            particleManager = this;
        }
        else if (particleManager != this)
            Destroy(gameObject);
    }
}
