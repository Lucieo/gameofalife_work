using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitAndDisappear : MonoBehaviour
{
    float disappearDelay=7;
    private float disappearTimer = 0;


    private void EraseObject()
    {
        if (disappearTimer >= disappearDelay)
        {
            Destroy(gameObject);
            disappearTimer = 0;
        }
        else
        {
            disappearTimer += Time.deltaTime;
        }
    }



    // Update is called once per frame
    void Update()
    {
        EraseObject();
    }
}
