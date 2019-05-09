using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disappearOutsideCamera : MonoBehaviour
{
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
