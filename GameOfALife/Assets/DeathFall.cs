using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathFall : Character
{
    private float lastYTravelDistance;
    private float lastYPosition;
    private float fallHeight;
    [SerializeField] private float deathHeight=10;

    private void ManageFall()
    {
        if (!Player.Instance.OnGround)
        {
            //calculate the distance between our current height and the height we were in the last frame
            lastYTravelDistance = Player.Instance.transform.position.y - lastYPosition;
            //if the difference is negative, it means we're descending 
            fallHeight += lastYTravelDistance < 0 ? lastYTravelDistance : 0;
            //Debug.Log("fallHeight: " + -fallHeight);
        }
        else{
            //we check to see if we passed the allowed falling distance and kill the player if necessary
            //Debug.Log("fallHeight: " + -fallHeight);
            //Debug.Log("deathHeight: " + deathHeight);
            if (-fallHeight >= deathHeight)
                Player.Instance.Death();
            //reset fall height since we landed (doesn't matter if we're dead or alive)
            fallHeight = 0;
        }
        lastYPosition = Player.Instance.transform.position.y;
    }

    // Start is called before the first frame update
    public override void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        ManageFall();
    }

    public override void AfterDeath() { }

    public override IEnumerator TakeDamage(int force)
    {
        yield return null;
    }

    public override bool IsDead
    {
        get
        {
            return false;
        }
    }

}
