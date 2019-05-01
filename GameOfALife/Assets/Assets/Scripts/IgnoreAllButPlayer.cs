using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreAllButPlayer : MonoBehaviour {
    private List<string> doNotIgnoreGameObjects = new List<string>(new string[] {
            "Player",
            "Floor",
            "MediumCoin"
    });

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string tag = collision.gameObject.tag;
        if (!doNotIgnoreGameObjects.Contains(tag))
        {
            // Debug.Log(this.tag + " should ignore " + tag);
            Collider2D[] m_Colliders = GetComponents<Collider2D>();
            Collider2D collider = collision.gameObject.GetComponent<Collider2D>();
            foreach (Collider2D m_Collider in m_Colliders)
            {
                Physics2D.IgnoreCollision(collider, m_Collider, true);
            }
        }
    }

}
