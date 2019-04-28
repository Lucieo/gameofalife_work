using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusMaker : MonoBehaviour
{
    [SerializeField] private GameObject bonusPrefab;
    [SerializeField] private float delay;
    private float bonusDelay = 5;
    private float bonusTimer = 0;


    private void CreateCookie()
    {
        if (bonusTimer >= bonusDelay)
        {
            GameObject bottle = (GameObject)Instantiate(bonusPrefab, new Vector3(transform.position.x+4, transform.position.y -2), Quaternion.identity);
            bonusTimer = 0;
        }
        else
        {
            bonusTimer += Time.deltaTime;
        }
    }



    // Update is called once per frame
    void Update()
    {
        CreateCookie();
    }
}
