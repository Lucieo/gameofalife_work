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
    [SerializeField] private GameObject coinPrefab;
    [SerializeField]
    protected Transform weaponSpawn;
    private int weaponIndex=0;
    private Vector3 startPos;
    [SerializeField] private Transform target;
    private bool isMovingUp;
    private bool isMovingDown;
    public float speed;
    public int BossLife;
    private Renderer rend;
    private bool hasDied=false;
    [SerializeField] private AudioSource musique;
    [SerializeField] private AudioSource flySound;
    [SerializeField] private AudioSource groanSound;

    // Start is called before the first frame update
    void Start()
    {
        DragonAnimator = GetComponent<Animator>();
        startPos = transform.position;
        rend = GetComponent<Renderer>();
        BossLife = 20;
    }

    private void Update()
    {
        FlyingBoss();

    }

    IEnumerator DragonRoutine()
    {
        while (!DragonAnimator.GetBool("isDead"))
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
            StartCoroutine(Flasher());
            if(BossLife>0){
                BossLife -= 10;
                Debug.Log(BossLife);
            }
            else if(!hasDied)
            {
                StopCoroutine("DragonRoutine");
                Collider2D collider=GetComponent<Collider2D>();
                collider.enabled = false;
                Debug.Log("has died");
                hasDied = true;
                DragonAnimator.SetBool("isDead", true);
                DragonAnimator.SetBool("isJumping", false);
                DragonAnimator.SetBool("isFlying", false);
                DragonAnimator.SetBool("isHurt", false);
                DragonAnimator.SetBool("isLanding", false);
                DragonAnimator.SetBool("isAttacking", false);
                transform.position = startPos;
                GameObject coin = (GameObject)Instantiate(coinPrefab, new Vector3(transform.position.x, transform.position.y - 1), Quaternion.identity);
            }
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

    IEnumerator Flasher()
    {
        for (int i = 0; i < 4; i++)
        {
            rend.enabled = false;
            yield return new WaitForSeconds(.1f);
            rend.enabled = true;
            yield return new WaitForSeconds(.1f);
        }
    }

    public void CollisionDetected(Collider2D childCollision)
    {
        groanSound.Play();
        StartCoroutine("DragonRoutine");
        transform.position = startPos;
        musique.Play();
    }



}
