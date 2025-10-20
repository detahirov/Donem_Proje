using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    Vector3 originalLocalPos;

    void Awake()
    {
        originalLocalPos = transform.localPosition;
    }

    public void Shake(float intensity, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(DoShake(intensity, duration));
    }

    IEnumerator DoShake(float intensity, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float damper = 1f - (t / duration);
            transform.localPosition = originalLocalPos + Random.insideUnitSphere * intensity * damper;
            yield return null;
        }
        transform.localPosition = originalLocalPos;
    }
}
