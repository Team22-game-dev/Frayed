using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
    public bool onFire { get; private set; }
    private bool inContact = false;

    [SerializeField]
    private float timeToIgnite = 2f; // Time in contact to ignite object
    [SerializeField]
    private float burnTime = 5f; // Time this object burns for
    [SerializeField]
    private GameObject firePrefab;

    private float startTime = -1f; // Time fire contact started
    private GameObject newFire; // Reference to the fire object

    void Update()
    {
        if (inContact && !onFire)
        {
            if (Time.time - startTime > timeToIgnite)
                Ignite();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("fire entered " + gameObject.name + "'s collider");
        if (onFire)
            return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Fire"))
        {
            startTime = Time.time;
            inContact = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Fire"))
        {
            inContact = false;
        }
    }

    private void Ignite()
    {
        onFire = true;
        inContact = false;

        Quaternion rotation = Quaternion.Euler(new Vector3(-90, 0, 0));


        if (firePrefab != null)
        {
            newFire = Instantiate(firePrefab, transform.position, rotation, transform);
        }

        StartCoroutine(BurnOut());
    }

    private IEnumerator BurnOut()
    {
        yield return new WaitForSeconds(burnTime);

        if (newFire != null)
        {
            Destroy(newFire);
        }

        Destroy(transform.root.gameObject); // Optional: remove this line if object shouldn't disappear after burning
    }
}
