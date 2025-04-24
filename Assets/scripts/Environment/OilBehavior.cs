using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilBehavior : MonoBehaviour
{
    public void SetOilPrefab(GameObject prefab) => oilPrefab = prefab;
    private int hierarchy = -1;

    private bool set = false;

    [SerializeField] private GameObject oilPrefab;
    // [SerializeField] private int hierarchyLimit = 2;
    [SerializeField] private int childLimit = 3;
    [SerializeField] private float noiseStrength = 20f;
    [SerializeField] private float spreadInterval = 0.3f;
    [SerializeField] private float spreadDistance = 2f;
    [SerializeField] private float spreadDuration = 0.8f;

    private Vector3 flowDirection;
    private bool isSpreading = false;

    void Update()
    {
        if (!set)
        {
            StartCoroutine(WaitToStart());
            set = true;
        }
    }

    private IEnumerator WaitToStart()
    {
        yield return new WaitForSeconds(1f);
        if (hierarchy == -1)
        {
            hierarchy = 0;
            BeginSpread();
        }
    }

    public void SetIsChild(int value) => hierarchy = value;

    public void BeginSpread()
    {
        if (!isSpreading)
        {
            Debug.Log(gameObject.name + " Spreading from hierarchy level: " + hierarchy);
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
        }
    }

    private IEnumerator SpreadOil()
    {
        isSpreading = true;
        Vector3 lastPos = transform.position;

        for (int i = 0; i < childLimit; i++)
        {
            yield return new WaitForSeconds(spreadInterval);

            Vector3 direction = CalculateSpreadDirection(); // ✅ Now dynamically downhill
            Vector3 targetPos = lastPos + direction * spreadDistance;

            RaycastHit hit;
            if (Physics.Raycast(targetPos + Vector3.up, Vector3.down, out hit, 2f,
                1 << LayerMask.NameToLayer("Terrain")))
            {
                Vector3 surfaceNormal = hit.normal;
                Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, surfaceNormal);
                Vector3 finalPos = hit.point + surfaceNormal * 0.01f;

                Debug.Log($"Start: {lastPos}, Final: {finalPos}, Distance: {Vector3.Distance(lastPos, finalPos)}");

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
        }
    }

    /// ✅ New and improved downhill-aware direction finder
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
            if (Physics.Raycast(samplePos, Vector3.down, out RaycastHit hit, 3f, 1 << LayerMask.NameToLayer("Terrain")))
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
            bestDirection = Vector3.down;
        }

        // Optional wobble for natural variance
        Quaternion wobble = Quaternion.AngleAxis(
            Random.Range(-noiseStrength, noiseStrength),
            Vector3.Cross(bestDirection, Vector3.up).normalized
        );

        return (wobble * bestDirection).normalized;
    }

    private IEnumerator AnimateOilDrop(GameObject oil, Vector3 startPos, Vector3 endPos)
    {
        float elapsed = 0f;
        while (elapsed < spreadDuration)
        {
            float t = elapsed / spreadDuration;
            t = t * t * (3f - 2f * t); // Smooth step
            oil.transform.position = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        oil.transform.position = endPos;
    }

    private IEnumerator DelayedChildSpread(OilBehavior childScript, float delay)
    {
        yield return new WaitForSeconds(delay);
        childScript.BeginSpread();
    }
}
