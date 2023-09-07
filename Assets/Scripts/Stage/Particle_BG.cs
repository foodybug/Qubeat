using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle_BG : MonoBehaviour
{
    #region - singleton -
    static Particle_BG instance;
    public static Particle_BG Instance
    { get { return instance; } }
    #endregion
    #region - member -
    [SerializeField] ParticleSystem particleBG;

    [SerializeField] float minLifeTime = 0.2f;
    [SerializeField] float maxLifeTime = 2f;

    [SerializeField] int minRate = 99;
    [SerializeField] int maxRate = 3333;

    [SerializeField] float m_MoveSpeedRatio = 1f;
    Vector3 m_Direction;
    #endregion
    #region - init & update -
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        StartCoroutine("RefreshParticle");
    }

    IEnumerator RefreshParticle()
    {
        float refreshTime = 0.2f;

        ParticleSystem.MainModule main = particleBG.main;
        ParticleSystem.EmissionModule emission = particleBG.emission;

        while (true)
        {
            yield return new WaitForSeconds(refreshTime);

            Player player = YEntityManager.Instance.PlayerEntity;
            if (player != null)
            {
                float ratio = Mathf.Pow(player.Exp * 0.01f, 1.5f);

                main.startLifetime = Mathf.Lerp(maxLifeTime, minLifeTime, ratio);
                emission.rateOverTime = Mathf.Lerp(minRate, maxRate, ratio);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (YEntityManager.Instance.PlayerEntity == null)
            return;

        if (YEntityManager.Instance.PlayerEntity.Living == false)
            return;

        Vector3 delta = -m_Direction * m_MoveSpeedRatio * Time.deltaTime;
        if(particleBG != null)
            particleBG.transform.position += delta;

        if (particleBG.transform.position.sqrMagnitude > 999f)
            particleBG.transform.position = Vector3.zero;
    }
    #endregion
    #region - public -
    public void FlowProc(Vector3 dir)
    {
        m_Direction = dir;
    }

    public void Reset()
    {
        if (particleBG != null)
            particleBG.transform.position = Vector3.zero;
    }
    #endregion
}
