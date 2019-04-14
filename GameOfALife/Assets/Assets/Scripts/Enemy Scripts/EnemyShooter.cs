using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : MonoBehaviour {

    [SerializeField]private GameObject bulletPrefab;

    // Use this for initialization
    void Start () {
        StartCoroutine(FireRepeating());
    }

    // Update is called once per frame
    void Update () {
		
	}

    private IEnumerator FireRepeating()
    {
        while(true)
        {
            //Random time
            int randomTime = Random.Range(10, 30);
            int stepSize = 5;
            int numSteps = (int) randomTime / stepSize;
            int adjustedTime = numSteps * stepSize;

            GameObject temp = (GameObject)Instantiate(bulletPrefab, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
            temp.GetComponent<Bullet>().Initialize(Vector2.left);

            yield return new WaitForSeconds(adjustedTime);
        }
    }
}
