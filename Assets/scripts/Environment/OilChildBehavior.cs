using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilChildBehavior : MonoBehaviour
{
    private int hierarchy = 10;
    private bool hierarchySet = false;

    [Header("Spreading Settings")]
    [SerializeField] private GameObject oilPrefab;
    [SerializeField] private int hierarchyLimit = 3;
    [SerializeField] private int childLimit = 3;
    [SerializeField] private float noiseStrength = 30f; // Reduced from 200 for more subtle wobble
    [SerializeField] private float spreadInterval = 0.3f;
    [SerializeField] private float spreadDistance = 10f; // More reasonable default distance
    [SerializeField] private float spreadDuration = 2.8f;

    private Vector3 flowDirection;
    private bool isSpreading = false;

    public IEnumerator WaitToStart()
    {
        yield return new WaitUntil(() => hierarchySet);
        if (hierarchy < hierarchyLimit)
        {
            BeginSpread();
        }
    }

    public void SetIsChild(int value)
    {
        hierarchy = value;
        hierarchySet = true;
        Debug.Log($"Set hierarchy to {value} for {gameObject.name}");
    }

    public void BeginSpread()
    {
        if (!isSpreading)
        {
            Debug.Log($"Beginning spread from {gameObject.name} (hierarchy {hierarchy})");
            SetUpVector();
            StartCoroutine(SpreadOil());
        }
    }

    private void SetUpVector()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
        int groundLayerMask = 1 << LayerMask.NameToLayer("Terrain");

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 2f, groundLayerMask))
        {
            Vector3 surfaceNormal = hit.normal;
            flowDirection = Vector3.ProjectOnPlane(Vector3.down, surfaceNormal).normalized;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, surfaceNormal);
            transform.position = hit.point + surfaceNormal * 0.01f;

            // Debug visualization
            Debug.DrawRay(transform.position, flowDirection * 2f, Color.blue, 5f);
            Debug.DrawRay(transform.position, surfaceNormal * 2f, Color.green, 5f);
        }
    }

    private IEnumerator SpreadOil()
    {
        isSpreading = true;
        Vector3 lastPos = transform.position;

        for (int i = 0; i < childLimit; i++)
        {
            yield return new WaitForSeconds(spreadInterval);

            Vector3 direction = CalculateSpreadDirection();
            Vector3 targetPos = lastPos + direction * spreadDistance;

            // Debug visualization of spread direction
            Debug.DrawRay(lastPos, direction * spreadDistance, Color.red, spreadInterval * 2f);

            RaycastHit hit;
            if (Physics.Raycast(targetPos + Vector3.up * 2f, Vector3.down, out hit, 3f,
                1 << LayerMask.NameToLayer("Terrain")))
            {
                Vector3 surfaceNormal = hit.normal;
                Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, surfaceNormal);
                Vector3 finalPos = hit.point + surfaceNormal * 0.01f;

                Debug.Log($"Oil spread from {lastPos} to {finalPos} (distance: {Vector3.Distance(lastPos, finalPos):F2})");

                GameObject newOil = Instantiate(oilPrefab, lastPos, targetRotation);
                StartCoroutine(AnimateOilDrop(newOil, lastPos, finalPos));

                OilChildBehavior childScript = newOil.GetComponent<OilChildBehavior>();
                if (childScript != null)
                {
                    int newHierarchy = hierarchy + 1;
                    childScript.SetIsChild(newHierarchy);
                    childScript.StartCoroutine(childScript.WaitToStart());
                }

                lastPos = finalPos;
            }
            else
            {
                Debug.LogWarning($"No terrain found at target position {targetPos}");
            }
        }

        isSpreading = false;
    }

    private Vector3 CalculateSpreadDirection()
    {
        Vector3[] offsets =
        {
            Vector3.forward,
            Vector3.back,
            Vector3.left,
            Vector3.right,
            (Vector3.forward + Vector3.right).normalized,
            (Vector3.forward + Vector3.left).normalized,
            (Vector3.back + Vector3.right).normalized,
            (Vector3.back + Vector3.left).normalized
        };

        Vector3 origin = transform.position + Vector3.up * 0.5f;
        float bestHeight = float.MaxValue;
        Vector3 bestDirection = Vector3.zero;

        foreach (Vector3 offset in offsets)
        {
            Vector3 samplePos = origin + offset * 1f;
            if (Physics.Raycast(samplePos, Vector3.down, out RaycastHit hit, 3f, 
                1 << LayerMask.NameToLayer("Terrain")))
            {
                if (hit.point.y < bestHeight)
                {
                    bestHeight = hit.point.y;
                    bestDirection = (hit.point - transform.position).normalized;
                }
            }
        }

        if (bestDirection == Vector3.zero)
        {
            bestDirection = flowDirection.normalized;
            if (bestDirection == Vector3.zero)
            {
                bestDirection = Vector3.down;
            }
        }

        // Calculate wobble with safer axis
        Vector3 rotationAxis = Vector3.Cross(bestDirection, Vector3.up);
        if (rotationAxis.magnitude < 0.1f)
        {
            rotationAxis = Vector3.Cross(bestDirection, Vector3.forward);
        }

        Quaternion wobble = Quaternion.AngleAxis(
            Random.Range(-noiseStrength, noiseStrength),
            rotationAxis.normalized
        );

        Vector3 finalDirection = (wobble * bestDirection).normalized;
        Debug.DrawRay(transform.position, finalDirection * 1.5f, Color.magenta, spreadInterval);
        return finalDirection;
    }

    private IEnumerator AnimateOilDrop(GameObject oil, Vector3 startPos, Vector3 endPos)
    {
        float elapsed = 0f;
        while (elapsed < spreadDuration)
        {
            float t = elapsed / spreadDuration;
            t = t * t * (3f - 2f * t); // Smooth step interpolation
            oil.transform.position = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        oil.transform.position = endPos;
    }
}