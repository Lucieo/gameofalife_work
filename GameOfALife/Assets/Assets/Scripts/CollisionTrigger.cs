 using UnityEngine;
using System.Collections;

public class CollisionTrigger : MonoBehaviour {
     
    [SerializeField]
    private BoxCollider2D platformCollider;
    [SerializeField]
    private BoxCollider2D platformTrigger;

	// Use this for initialization
	void Start () {
        Physics2D.IgnoreCollision(platformCollider, platformTrigger, true);
	}

    void OnTriggerEnter2D(Collider2D other) {
        //if player collides with platform
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy") {
            //ignore collision 
            Physics2D.IgnoreCollision(platformCollider, other, true);
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        //if player stops colliding then stop ignoring collision
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy") {
            Physics2D.IgnoreCollision(platformCollider, other, false);
        }
    }
}
