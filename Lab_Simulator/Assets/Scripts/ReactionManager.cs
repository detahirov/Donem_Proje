using System.Collections;
using UnityEngine;

public class ReactionManager : MonoBehaviour
{
    public static ReactionManager Instance { get; private set; }

    [Header("Default VFX/Audio (assign prefabs/clips)")]
    public ParticleSystem defaultExplosionVFX;
    public ParticleSystem defaultSteamVFX;
    public ParticleSystem defaultBubbleVFX;
    public AudioClip defaultFizz;
    public AudioClip defaultBang;

    [Header("Global reaction params")]
    public float cameraShakeIntensity = 0.2f;
    public float cameraShakeDuration = 0.4f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void StartReaction(Chemical chem, WaterContainer container, Vector3 contactPoint)
    {
        StartCoroutine(DoReactionCoroutine(chem, container, contactPoint));
    }

    IEnumerator DoReactionCoroutine(Chemical chem, WaterContainer container, Vector3 point)
    {
        // Choose VFX/audio
        ParticleSystem expl = chem.explosionVFX ? chem.explosionVFX : defaultExplosionVFX;
        ParticleSystem steam = chem.steamVFX ? chem.steamVFX : defaultSteamVFX;
        ParticleSystem bubbles = chem.bubbleVFX ? chem.bubbleVFX : defaultBubbleVFX;
        AudioClip fizz = chem.fizzAudio ? chem.fizzAudio : defaultFizz;
        AudioClip bang = chem.bangAudio ? chem.bangAudio : defaultBang;

        // small pre-fizz
        if (fizz != null) AudioSource.PlayClipAtPoint(fizz, point, 0.8f);

        yield return new WaitForSeconds(0.15f);

        // spawn bubbles inside water
        if (bubbles != null)
        {
            var b = Instantiate(bubbles, point, Quaternion.identity);
            b.Play();
            Destroy(b.gameObject, 4f);
        }

        // steam/gas
        if (steam != null)
        {
            var s = Instantiate(steam, point, Quaternion.identity);
            s.Play();
            Destroy(s.gameObject, 6f);
        }

        // explosion VFX & sound
        if (expl != null)
        {
            var e = Instantiate(expl, point, Quaternion.identity);
            e.Play();
            Destroy(e.gameObject, 6f);
        }
        if (bang != null) AudioSource.PlayClipAtPoint(bang, point, 1f);

        // apply physics explosion
        float force = chem.explosionForce;
        float radius = chem.explosionRadius;

        Collider[] cols = Physics.OverlapSphere(point, radius);
        foreach (var c in cols)
        {
            Rigidbody rb = c.attachedRigidbody;
            if (rb != null && !rb.isKinematic)
            {
                rb.AddExplosionForce(force, point, radius);
            }
        }

        // camera shake
        var mainCam = Camera.main;
        if (mainCam != null)
        {
            var shake = mainCam.GetComponent<CameraShake>();
            if (shake != null)
            {
                shake.Shake(cameraShakeIntensity, cameraShakeDuration);
            }
        }

        yield return null;
    }
}
