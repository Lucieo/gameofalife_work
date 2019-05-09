using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterZone : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Player"){
            transform.parent.GetComponent<BigBossManager>().CollisionDetected(col);
        }
    }
}
