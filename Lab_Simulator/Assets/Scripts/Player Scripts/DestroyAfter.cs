using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public float time = 3f;
    void Start() { Destroy(gameObject, time); }
}
