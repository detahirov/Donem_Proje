// Assets/Chemistry/Scripts/Effects/EffectRoutines.cs
using System.Collections;
using UnityEngine;

public static class EffectRoutines
{
    public static IEnumerator LerpColor(Material mat, Color target, float time)
    {
        Color start = mat.color; float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            mat.color = Color.Lerp(start, target, t / time);
            yield return null;
        }
    }

    public static IEnumerator EmitGas(GameObject gasPrefab, Transform spawn, float rate, float duration)
    {
        float t = 0f; float interval = 1f / Mathf.Max(0.1f, rate);
        while (t < duration)
        {
            GameObject g = Object.Instantiate(gasPrefab, spawn.position, Quaternion.identity);
            Object.Destroy(g, 4f);
            t += interval;
            yield return new WaitForSeconds(interval);
        }
    }

    public static IEnumerator SpawnTimed(GameObject prefab, Vector3 pos, float duration)
    {
        var go = Object.Instantiate(prefab, pos, Quaternion.identity);
        yield return new WaitForSeconds(duration);
        Object.Destroy(go);
    }
}
