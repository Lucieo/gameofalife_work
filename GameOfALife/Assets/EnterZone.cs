using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterZone : MonoBehaviour
{
    [SerializeField]private GameObject limitLeft;
    [SerializeField] private Transform limitRightTarget;
    [SerializeField] private GameObject limitRight;
    [SerializeField] private Transform limitLeftTarget;
    [SerializeField] AudioSource gateSound;
    private bool isMovingUp=false;

    private void Update()
    {
        float step = 4 * Time.deltaTime;
        if (isMovingUp){
            if (limitLeft.transform.position == limitLeftTarget.position || limitRight.transform.position == limitRightTarget.position)
            {
                isMovingUp = false;
            }
            else
            {
                limitLeft.transform.position = Vector3.MoveTowards(limitLeft.transform.position, limitLeftTarget.position, step);
                limitRight.transform.position = Vector3.MoveTowards(limitRight.transform.position, limitRightTarget.position, step);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Player"){
            transform.parent.GetComponent<BigBossManager>().CollisionDetected(col);
            GetComponent<Collider2D>().enabled = false;
            limitLeft.SetActive(true);
            limitRight.SetActive(true);
            gateSound.Play();
            isMovingUp = true;
        }
    }
}
