using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollisionTrigger : MonoBehaviour {
     
    [SerializeField]
    private BoxCollider2D platformCollider;
    [SerializeField]
    private BoxCollider2D platformTrigger;

    private List<string> ignoreGameObjects = new List<string>(new string[] {
            "Player",
            "Floor"
    });

    // Use this for initialization
    void Start () {
        Physics2D.IgnoreCollision(platformCollider, platformTrigger, true);
	}

    void OnTriggerEnter2D(Collider2D other) {
        //if player collides with platform
        if (ignoreGameObjects.Contains(other.gameObject.tag)) {
            //ignore collision 
            Physics2D.IgnoreCollision(platformCollider, other, true);
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        //if player stops colliding then stop ignoring collision
        if (ignoreGameObjects.Contains(other.gameObject.tag)) {
            Physics2D.IgnoreCollision(platformCollider, other, false);
        }
    }
}
