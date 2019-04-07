using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bath : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter");
    }

    void OnTriggerStay(Collider other)
    {
        Debug.Log("Stay");
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("Exit");
    }
}
