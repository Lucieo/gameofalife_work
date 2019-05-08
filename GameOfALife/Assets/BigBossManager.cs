using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBossManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] weapons;
    public Animator DragonAnimator { get; private set; }
    [SerializeField]
    protected GameObject weaponPrefab;
    [SerializeField]
    protected Transform weaponSpawn;
    private int weaponIndex=0;
    private Vector3 startPos;
    [SerializeField] private Transform target;
    private bool isMovingUp;
    private bool isMovingDown;
    public float speed;
    public int BossLife=100;
    public Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        DragonAnimator = GetComponent<Animator>();
        StartCoroutine(DragonRoutine());
        startPos = transform.position;
        rend = GetComponent<Renderer>();
    }

    private void Update()
    {
        FlyingBoss();
        if(DragonAnimator.GetBool("isHurt")){
            BlinkBoss(2);
        }

    }

    IEnumerator DragonRoutine()
    {
        while (true)
        {
            //Attack
            yield return new WaitForSeconds(Random.Range(0.5f, 2f));
            DragonAnimator.SetBool("isAttacking", true);
            yield return new WaitForSeconds(Random.Range(0.5f, 2f));
            DragonAnimator.SetBool("isAttacking", true);
            yield return new WaitForSeconds(Random.Range(0.5f, 2f));
            DragonAnimator.SetBool("isAttacking", true);
            yield return new WaitForSeconds(3);

            //Take off
            isMovingUp = true;
            DragonAnimator.SetBool("isJumping", true);
            yield return new WaitForSeconds(4);

            //Send objects from sky
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
            yield return new WaitForSeconds(10);
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
            isMovingDown = true;
            yield return new WaitForSeconds(4);

            //Land
            DragonAnimator.SetBool("isJumping", false);
            DragonAnimator.SetBool("isLanding", true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.tag == "Sword" || other.gameObject.tag=="Knife"){
            DragonAnimator.SetBool("isHurt", true);
            BossLife -= 10;        
        }
    }

    private void throwObject(){
        GameObject temp = (GameObject)Instantiate(weapons[weaponIndex], weaponSpawn.position, Quaternion.Euler(new Vector3(0, 0, +90)));
        temp.GetComponent<Knife>().Initialize(Vector2.left);
        if(weaponIndex==weapons.Length-1){
            weaponIndex = 0;
        }
        else{
            weaponIndex += 1;
        }
        DragonAnimator.SetBool("isAttacking", false);

    }

    private void FlyingBoss(){
        float step = speed * Time.deltaTime;
        if (isMovingUp)
        {
            if (transform.position == target.position)
            {
                isMovingUp = false;
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, step);
            }
        }
        if (isMovingDown)
        {
            if (transform.position == startPos)
            {
                isMovingDown = false;
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, startPos, step);
            }
        }
    }

    private void BlinkBoss(int numBlink)
    {
        for (int i = 0; i < numBlink; i++)
        {
            rend.enabled = false;
            StartCoroutine(Wait(.2f));
            rend.enabled = true;
            StartCoroutine(Wait(.2f));
        }
    }

    IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

}
