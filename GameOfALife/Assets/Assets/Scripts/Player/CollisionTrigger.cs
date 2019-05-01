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
            "Floor",
    });

    // Use this for initialization
    void Start () {
        Physics2D.IgnoreCollision(platformCollider, platformTrigger, true);
	}

    void OnTriggerEnter2D(Collider2D other) {
        string otherTag = other.gameObject.tag;
        //if player collides with platform
        if (ignoreGameObjects.Contains(otherTag)) {
            //ignore collision 
            Physics2D.IgnoreCollision(platformCollider, other, true);
        }

        if (otherTag == "Player") {
            Player.Instance.IgnoreGround = true;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        string otherTag = other.gameObject.tag;
        //if player stops colliding then stop ignoring collision
        if (ignoreGameObjects.Contains(otherTag)) {
            Physics2D.IgnoreCollision(platformCollider, other, false);
        }

        if (otherTag == "Player") {
            Player.Instance.IgnoreGround = false;
        }
    }
}
