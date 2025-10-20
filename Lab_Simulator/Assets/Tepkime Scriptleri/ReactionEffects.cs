using UnityEngine;

public class ReactionEffects : MonoBehaviour
{
    public ParticleSystem ps;
    public float life = 5f;
    void Start()
    {
        if (ps != null) ps.Play();
        Destroy(gameObject, life);
    }
}
