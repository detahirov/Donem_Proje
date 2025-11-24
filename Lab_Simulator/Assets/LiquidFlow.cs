using UnityEngine;

public class LiquidFlow : MonoBehaviour
{
    [Header("Liquid Amount")]
    public float maxAmount = 100f;     // toplam sıvı
    public float amount = 100f;        // kalan sıvı
    public float pourRate = 20f;       // saniyede ne kadar azalsın

    [Header("Visual")]
    public ParticleSystem pourParticles;   // PourPoint altındaki particle
    public Transform liquidMesh;          // içteki Liquid child'ı (opsiyonel)

    bool isPouring;

    void Awake()
    {
        if (pourParticles != null)
        {
            var emission = pourParticles.emission;
            emission.enabled = false;
            pourParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        if (maxAmount <= 0f) maxAmount = amount > 0 ? amount : 100f;
        amount = Mathf.Clamp(amount, 0f, maxAmount);
    }

    void Update()
    {
        if (!isPouring || amount <= 0f)
            return;

        float poured = pourRate * Time.deltaTime;
        amount = Mathf.Max(0f, amount - poured);

        // tamamen boşaldıysa partikülü durdur
        if (amount <= 0f)
        {
            StopPour();

            // istersek Liquid mesh'ini de yok edelim
            if (liquidMesh != null)
                liquidMesh.gameObject.SetActive(false);
        }
    }

    // Dışarıdan (BottleController'dan) çağıracağın fonksiyonlar:

    public void StartPour()
    {
        if (amount <= 0f) return;

        isPouring = true;

        if (pourParticles != null)
        {
            var emission = pourParticles.emission;
            emission.enabled = true;
            emission.rateOverTime = pourRate;   // ne kadar hızlı boşaldığına göre

            if (!pourParticles.isPlaying)
                pourParticles.Play();
        }
    }

    public void StopPour()
    {
        isPouring = false;

        if (pourParticles != null)
        {
            var emission = pourParticles.emission;
            emission.enabled = false;

            if (pourParticles.isPlaying)
                pourParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}
