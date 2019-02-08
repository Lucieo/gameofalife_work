using UnityEngine;
using System.Collections;

public class IgnoreCollision : MonoBehaviour {
    [SerializeField]
    private Collider2D other;

    void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }
}
