using UnityEngine;
using System.Collections.Generic;

public class OilSplashHandler : MonoBehaviour
{
    public GameObject oilDecalPrefab; // assign your decal prefab
    private ParticleSystem ps;
    private List<ParticleCollisionEvent> collisionEvents;

    public List<GameObject> oilDecals { get; private set; }

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
        oilDecals = new List<GameObject>();
    }

    void OnParticleCollision(GameObject other)
    {
        int eventsCount = ps.GetCollisionEvents(other, collisionEvents);

        for (int i = 0; i < eventsCount; i++)
        {
            Vector3 hitPoint = collisionEvents[i].intersection;
            Vector3 normal = collisionEvents[i].normal;

            // Place the oil decal (or puddle)
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, normal);
            oilDecals.Add(Instantiate(oilDecalPrefab, hitPoint + normal * 0.01f, rot)); // tiny offset to avoid z-fighting
        }
    }
}
