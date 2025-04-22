using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilFire : MonoBehaviour
{
    private EnemyData enemyData;
    private GameObject fire;
    private ParticleSystem fireParticle;
    private ParticleSystem fireAddParticle;
    private ParticleSystem glowParticle;
    [SerializeField]
    private float timeOnFire;
    private float radius;
    private CapsuleCollider fireCollider;

    [SerializeField]
    private float maxRadius = 6.0f;
    
    public bool onFire { get { return fire.activeSelf; } }

    void Start()
    {
        enemyData = GetComponent<EnemyData>();
        Debug.Assert(enemyData != null);

        fire = this.transform.Find("Fire").gameObject;
        Debug.Assert(fire != null);

        fireParticle = fire.transform.Find("Fire").GetComponent<ParticleSystem>();
        Debug.Assert(fireParticle != null);

        fireAddParticle = fire.transform.Find("Fire_ADD").GetComponent<ParticleSystem>();
        Debug.Assert(fireAddParticle != null);

        glowParticle = fire.transform.Find("glow").GetComponent<ParticleSystem>();
        Debug.Assert(glowParticle != null);

        fireCollider = fire.GetComponent<CapsuleCollider>();
        Debug.Assert(fireCollider != null);
        fireCollider.isTrigger = true;

        timeOnFire = 0.0f;
        radius = 0.0f;
        fire.gameObject.SetActive(false);
    }

    void Update()
    {
        if (onFire)
        {
            timeOnFire += Time.deltaTime;
            radius = Mathf.Min(Mathf.Pow(timeOnFire, 1.0f / 4.0f) * 2.0f, maxRadius);
            ParticleSystem.ShapeModule fireParticleShape = fireParticle.shape;
            fireParticleShape.radius = radius;
            ParticleSystem.ShapeModule fireAddParticleShape = fireAddParticle.shape;
            fireAddParticleShape.radius = radius;
            ParticleSystem.ShapeModule glowParticleShape = glowParticle.shape;
            glowParticleShape.radius = radius;
            fireCollider.radius = radius;
        }
        else
        {
            if (Mathf.Approximately(enemyData.GetHealthRatio(), 0.0f))
            {
                ActivateFire();
            }
        }
    }

    private void ActivateFire()
    {
        timeOnFire = 0.0f;
        radius = 0.5f;
        fire.gameObject.SetActive(true);
    }
}
