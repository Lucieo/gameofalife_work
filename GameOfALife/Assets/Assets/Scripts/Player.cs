﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public delegate void DeadEventHandler();

public class Player : Character {
    private static Player instance;
    public event DeadEventHandler Dead;

    [SerializeField]private Stat healthStat;

    public static Player Instance {
        get {
            if (instance == null) {
                instance = GameObject.FindObjectOfType<Player>();
            }
            return instance; 
        }
    }

    
    [SerializeField]
    private Transform[] groundPoints;
    [SerializeField]
    private float groundRadius;

    [SerializeField]
    private LayerMask whatIsGround;
    [SerializeField]
    private float jumpForce;

    [SerializeField]
    private bool airControl;

    public Rigidbody2D MyRigidbody { get; set; }
    private SpriteRenderer mSpriteRenderer { get; set; }
    public bool Jump { get; set; }
    public bool OnGround { get; set; }

   private Color originalColor;

    [SerializeField]
    private Vector3 startPos;

    private bool immortal = false;
    [SerializeField]
    private float immortalDuration;

    private bool isPoisoned =false;
    private float poisonedDelay = 5;
    private float poisonedTimer = 0;

    [SerializeField] AudioSource BackMusic;
    [SerializeField] AudioSource JumpSound;
    [SerializeField] AudioSource CollectibleSound;
    [SerializeField] AudioSource DeadSound;
    [SerializeField] AudioSource Throw;
    [SerializeField] AudioSource Hurt;
    [SerializeField] AudioSource CollectCoin;
    [SerializeField] AudioSource ThrowPlayer;
    [SerializeField] AudioSource WinLevel;

    private Vector2 crouchSize = new Vector2(1.63f, 1.8f);
    private Vector2 crouchOffset = new Vector2(0f, -1.3f);
    private Vector2 normalSize;

    private void Awake()
    {
        healthStat.Initialize();
    }

    public override void Start () {
        base.Start();
        MyRigidbody = GetComponent<Rigidbody2D>();
        mSpriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = mSpriteRenderer.material.color;
        BackMusic.Play();
        normalSize = GetComponent<BoxCollider2D>().size;
    }

    void Update() {
        if (!TakingDamage && !IsDead) {
            if (transform.position.y <= -14f) {
                Death();
            }
            HandleInput();
        }

        // Easeier pausing
        if (Input.GetKeyDown(KeyCode.Q)) {
            Debug.Break();
        }

        //RemoveLife if poisonned
        if(isPoisoned && !IsDead)
        {
            if ( poisonedTimer >= poisonedDelay){
                TakeDamage();
                poisonedTimer = 0;
            }
            else{
                poisonedTimer += Time.deltaTime;
            }
        }

    }

	void FixedUpdate () {
        if (!TakingDamage && !IsDead) {
            float hInput = Input.GetAxis("Horizontal");

            OnGround = IsGrounded();

            HandleMovement(hInput);
            Flip(hInput);

            HandleLayers();
        }
	}

    public void OnDead() {
        if (Dead != null) {
            Dead();
        }
    }

    private void HandleMovement(float hInput) {
        // If we aren't moving up or down, we're on the ground.
        // (Not actually true but lolololYouTubeTutorials
        if (MyRigidbody.velocity.y < 0) {
            mAnimator.SetBool("land", true);
        }

        // If we aren't attacking, or sliding, and are either on the ground or in the air, we move.
        if (!Attack && (OnGround || airControl)) {
            if(isPoisoned)
            {
                MyRigidbody.velocity = new Vector2(hInput * movementSpeed * -1, MyRigidbody.velocity.y);
            }
            else{
                MyRigidbody.velocity = new Vector2(hInput * movementSpeed, MyRigidbody.velocity.y);
            }
        }

        // If we want to jump and we aren't currently falling, add jump force
        if (Jump && MyRigidbody.velocity.y == 0) {
            MyRigidbody.AddForce(new Vector2(0, jumpForce));
        }

        mAnimator.SetFloat("speed", Mathf.Abs(hInput));

    }

    // Checks inputs
    private void HandleInput() {
        if (Input.GetButtonDown("Fire1")) {
            mAnimator.SetTrigger("attack");
        }

        if (Input.GetButtonDown("Jump")) {
            mAnimator.SetTrigger("jump");
            JumpSound.Play();
        }
        if (Input.GetButtonDown("Fire2")) {
            mAnimator.SetTrigger("throw");
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            mAnimator.SetBool("crouch", true);
            GetComponent<BoxCollider2D>().size=crouchSize;
            GetComponent<BoxCollider2D>().offset = crouchOffset;
        }
        else
        {
            mAnimator.SetBool("crouch", false);
            GetComponent<BoxCollider2D>().size = normalSize;
            GetComponent<BoxCollider2D>().offset = new Vector2(0f,0f);
        }
    }

    private void Flip(float hInput) {
        if (!mAnimator.GetCurrentAnimatorStateInfo(0).IsTag("attack")) {
            if ((hInput > 0 && !facingRight || hInput < 0 && facingRight) && !isPoisoned) {
                ChangeDirection();
            }
            else if ((hInput < 0 && !facingRight || hInput > 0 && facingRight) && isPoisoned)
            {
                ChangeDirection();
            }
        }
    }

    private bool IsGrounded() {
        if (MyRigidbody.velocity.y <= 0) {
            foreach (Transform point in groundPoints) {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, groundRadius, whatIsGround);

                for (int i = 0; i < colliders.Length; i++) {
                    if (colliders[i].gameObject != gameObject) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void HandleLayers() {
        if (!OnGround) {
            mAnimator.SetLayerWeight(1, 1);
        }

        else {
            mAnimator.SetLayerWeight(1, 0);
        }
    }

    public void KnifeAnimation(int value) {
        ThrowKnife(value);
    }
    public override void ThrowKnife(int value) {
        if (!OnGround && value == 1 || OnGround && value == 0) {
            base.ThrowKnife(value);
        }
        ThrowPlayer.Play();

    }

    public override IEnumerator TakeDamage() {
        if (!immortal) {
            healthStat.CurrentValue -= 10;

            if (!IsDead) {
                mAnimator.SetTrigger("damage");
                Hurt.Play();
                immortal = true;
                StartCoroutine(IndicateImmortal());
                yield return new WaitForSeconds(immortalDuration);

                immortal = false;

            }
            else {
                mAnimator.SetLayerWeight(1, 0);
                mAnimator.SetTrigger("death");
                BackMusic.Stop();
                DeadSound.Play();
            }
        }
    }

    private IEnumerator IndicateImmortal() {
        while (immortal) {
            mSpriteRenderer.enabled = false;
            yield return new WaitForSeconds(.1f);
            mSpriteRenderer.enabled = true;
            yield return new WaitForSeconds(.1f);
        }
    }

    public override bool IsDead {
        get {
            if (healthStat.CurrentValue <= 0) {
                OnDead();
            }
            return healthStat.CurrentValue <= 0;
        }
    }

    public override void Death() {
        MyRigidbody.velocity = Vector2.zero;
        mAnimator.SetTrigger("idle");
        healthStat.CurrentValue = healthStat.MaxValue;
        transform.position = startPos;
        BackMusic.Play();
        JumpSound.Play();
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        if (other.gameObject.tag == "MediumCoin")
        {
            GameManager.Instance.CollectedCoins+=25;
            Destroy(other.gameObject);
            CollectibleSound.Play();
        }
        else if (other.gameObject.tag == "SmallCoin")
        {
            GameManager.Instance.CollectedCoins+=10;
            Destroy(other.gameObject);
            CollectibleSound.Play();
        }
        else if (other.gameObject.tag == "Poison")
        {
            healthStat.CurrentValue -= 10;
            Destroy(other.gameObject);
            isPoisoned = true;
            movementSpeed = movementSpeed/2;
            mSpriteRenderer.material.color = Color.green;
            Invoke("CureOfPoison", 10);
        }
        else if (other.gameObject.tag == "LifePotion")
        {
            healthStat.CurrentValue += 10;
            Destroy(other.gameObject);
            CureOfPoison();
        }
        else if (other.gameObject.tag=="Endforest")
        {
            HasWonLevel();
            Invoke("ChangeCyrilScene2", 5);
        }
        else if (other.gameObject.tag == "Endtown")
        {
            HasWonLevel();
            Invoke("BackMainMenu", 5);
        }
    }

    private void CureOfPoison()
    {
        if(isPoisoned){
            //MyRigidbody.freezeRotation = true;
            isPoisoned = false;
            movementSpeed = 2 * movementSpeed;
            mSpriteRenderer.material.color = originalColor;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Coin")
        {
            GameManager.Instance.CollectedCoins += 50;
            Destroy(other.gameObject);
            CollectCoin.Play();
        }
    }

    private void ChangeCyrilScene2(){
        mAnimator.SetBool("win", false);
        SceneManager.LoadScene("CyrilNantes");
    }
    private void BackMainMenu(){
        mAnimator.SetBool("win", false);
        SceneManager.LoadScene("MainMenu");
    }
    private void HasWonLevel(){
        mAnimator.SetBool("win", true);
        immortal = true;
        BackMusic.Stop();
        WinLevel.Play();
    }
}


