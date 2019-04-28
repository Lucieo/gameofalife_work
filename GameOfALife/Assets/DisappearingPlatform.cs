using UnityEngine;
using System.Collections;

public class DisappearingPlatform : MonoBehaviour
{
    [SerializeField] private float delay;
    [SerializeField]
    private Vector3 startPos;
    private Quaternion startRot;

    void Start()
    {
       startPos = transform.position;
        startRot = transform.rotation;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log(other.gameObject.tag);
        if(other.gameObject.tag == "Player"){
            GetComponent<Rigidbody2D>().isKinematic = false;
            StartCoroutine(waitAndDestroy());

        }

    }

    public IEnumerator waitAndDestroy(){
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
        GetComponent<Rigidbody2D>().isKinematic = true;
        CreatePlatform();
    }

    private void CreatePlatform(){
        gameObject.transform.position = startPos;
        gameObject.transform.rotation = startRot;
        gameObject.SetActive(true);
    }

}

