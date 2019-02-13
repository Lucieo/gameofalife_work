using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreAllButPlayer : MonoBehaviour {
   private void OnCollisionEnter2D(Collision2D collision)
    {

        if(collision.gameObject.tag!="Player" && collision.gameObject.tag !="Floor" && gameObject.tag!=collision.gameObject.tag)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.gameObject.GetComponent<Collider2D>(), true);
        }
    }

}
