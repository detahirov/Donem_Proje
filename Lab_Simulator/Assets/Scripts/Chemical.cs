using UnityEngine;

public enum ReactionType { None, SodiumInWater /*, AcidBase, ColorChange ... */ }

[RequireComponent(typeof(Rigidbody))]
public class Chemical : MonoBehaviour
{
    [Header("Chemical data")]
    public string reagentName = "Sodium";
    public ReactionType reactionType = ReactionType.SodiumInWater;
    public float mass = 0.05f; // kg - gösterim amaçlý

    [Header("Reaction visuals/audio (assign in inspector)")]
    public ParticleSystem explosionVFX;
    public ParticleSystem steamVFX;
    public ParticleSystem bubbleVFX;
    public AudioClip fizzAudio;
    public AudioClip bangAudio;

    [Header("Reaction params")]
    public float explosionForce = 300f;
    public float explosionRadius = 3f;
    public float reactionDelay = 0.2f; // kabýn içinde biraz bekleme

    [HideInInspector] public bool hasReacted = false;

    // Called when placed into a water container trigger
    private void OnTriggerEnter(Collider other)
    {
        if (hasReacted) return;

        WaterContainer wc = other.GetComponentInParent<WaterContainer>();
        if (wc != null)
        {
            // Notify container (container will call ReactionManager)
            wc.AddChemical(this);
        }
    }

    // Safe call to perform reaction (ReactionManager calls this to mark reacted)
    public void MarkReacted()
    {
        hasReacted = true;
    }
}
