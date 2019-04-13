using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DropBottle : MonoBehaviour
{
    [SerializeField] private GameObject bottlePrefab;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Sword")
        {
            GameObject bottle = (GameObject)Instantiate(bottlePrefab, new Vector3(transform.position.x, transform.position.y-1), Quaternion.identity);
        }
    }
}
