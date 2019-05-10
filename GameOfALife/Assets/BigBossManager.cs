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
    [SerializeField] private GameObject bossNest;
    [SerializeField]
    protected Transform weaponSpawn;
    private int weaponIndex=0;
    [SerializeField] private Transform startPos;
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
    [SerializeField] private AudioSource introSound;
    [SerializeField] private AudioSource deadSound;
    [SerializeField] private AudioSource winSound;
    private Collider2D colliderDragon;
    private bool isIntro = true;
    [SerializeField] private GameObject limiteGauche;
    [SerializeField] private GameObject limiteDroite;
    [SerializeField] private GameObject dragonBase;


    // Start is called before the first frame update
    void Start()
    {
        DragonAnimator = GetComponent<Animator>();
        rend = GetComponent<Renderer>();
        BossLife = 100;
        colliderDragon=GetComponent<Collider2D>();
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
            //launchSound.Play();
            yield return new WaitForSeconds(Random.Range(0.5f, 2f));
            DragonAnimator.SetBool("isAttacking", true);
            //launchSound.Play();
            yield return new WaitForSeconds(Random.Range(0.5f, 2f));
            DragonAnimator.SetBool("isAttacking", true);
            //launchSound.Play();
            yield return new WaitForSeconds(3);

            //Take off
            isMovingUp = true;
            DragonAnimator.SetBool("isJumping", true);
            flySound.Play();
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
            if(BossLife>0){
                DragonAnimator.SetBool("isHurt", true);
                StartCoroutine(Flasher());
                BossLife -= 10;
                Debug.Log(BossLife);
            }
            else if(!hasDied)
            {
                hasDied = true;
                bossNest.GetComponent<Collider2D>().enabled = false;
                colliderDragon.enabled = false;
                StartCoroutine(Flasher());
                StopCoroutine("DragonRoutine");
                musique.Stop();
                deadSound.Play();
                DragonAnimator.SetBool("isDead", true);
                DragonAnimator.SetBool("isJumping", false);
                DragonAnimator.SetBool("isFlying", false);
                DragonAnimator.SetBool("isHurt", false);
                DragonAnimator.SetBool("isLanding", false);
                DragonAnimator.SetBool("isAttacking", false);
                GameObject coin = (GameObject)Instantiate(coinPrefab, new Vector3(transform.position.x, transform.position.y - 1), Quaternion.identity);
                winSound.Play();
                limiteDroite.SetActive(false);
                limiteGauche.SetActive(false);
                dragonBase.SetActive(false);
                transform.position = startPos.position;
            }
            Debug.Log(BossLife);
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
            if (transform.position == startPos.position)
            {
                isMovingDown = false;
                if(isIntro){
                    isIntro = false;
                    DragonAnimator.SetBool("isFlying", false);
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, startPos.position, step);
            }
        }
    }

    IEnumerator Flasher()
    {
        colliderDragon.enabled = false;
        for (int i = 0; i < 4; i++)
        {
            rend.enabled = false;
            yield return new WaitForSeconds(.1f);
            rend.enabled = true;
            yield return new WaitForSeconds(.1f);
        }
        colliderDragon.enabled = true;


    }

    public void CollisionDetected(Collider2D childCollision)
    {
        musique.Play();
        //GameObject limits = GetComponent<Transform>().Find("Limites").gameObject;
        introSound.Play();
        DragonAnimator.SetBool("isFlying", true);
        isMovingDown = true;
        StartCoroutine(IntroWait());
        //limits.SetActive(true);
    }

    IEnumerator IntroWait(){
        yield return new WaitForSeconds(10f);
        StartCoroutine("DragonRoutine");
    }


}
