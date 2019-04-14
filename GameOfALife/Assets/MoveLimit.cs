using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLimit : MonoBehaviour
{
    public int RightLimit = -1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (RightLimit > 0)
        {
            Vector3 localPosition = this.transform.localPosition;
            this.transform.localPosition = new Vector3(Mathf.Min(localPosition.x, RightLimit), localPosition.y, localPosition.z);
        }
    }
}
