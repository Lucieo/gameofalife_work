using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class NoamPlayer : Character {
    private static NoamPlayer instance;

    [SerializeField]private Stat healthStat;

    public static NoamPlayer Instance {
        get {
            if (instance == null) {
                instance = GameObject.FindObjectOfType<NoamPlayer>();
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
    public bool Jump { get; set; }
    public bool OnGround { get; set; }

    private Color originalColor;

    [SerializeField]
    private Vector3 startPos;

    private bool win = false;
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
    [SerializeField] AudioSource ThrowNoamPlayer;
    [SerializeField] AudioSource WinLevel;
    [SerializeField] AudioSource LifePotion;
    [SerializeField] AudioSource Splash;
    [SerializeField] AudioSource Water;

    private SpriteRenderer Bath;
    private Animator[] BathAnimators;
    private List<GameObject> Garbage;

    private void Awake()
    {
        healthStat.Initialize();
    }

    public override void Start () {
        base.Start();
        MyRigidbody = GetComponent<Rigidbody2D>();
        originalColor = mSpriteRenderer.material.color;
        BackMusic.Play();
        Bath = GameObject.FindGameObjectWithTag("Bath").GetComponent<SpriteRenderer>();

        BathAnimators = new Animator[2];
        GameObject[] splashes = GameObject.FindGameObjectsWithTag("Splash");
        int i = 0;
        foreach(GameObject splash in splashes) {
            BathAnimators[i++] = splash.GetComponent<Animator>();
        }

        Garbage = new List<GameObject>();
    }

    void Update() {
        if (!TakingDamage && !IsDead) {
            if (transform.position.y <= -14f) {
                //AfterDeath();
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
        Rapetisser();
        Grow();

        if (!mAnimator.GetBool("pipe"))
        {
            if (!TakingDamage && !IsDead && !win)
            {
                float hInput = Input.GetAxis("Horizontal");

                OnGround = IsGrounded2D();

                HandleMovement(hInput, 0);
                Flip(hInput);

                HandleLayers();
            }
            else
            {
                HandleMovement(0, 0);
            }
        } else
        {
            float hInput = Input.GetAxis("Horizontal");
            float vInput = Input.GetAxis("Vertical");
            HandleMovement(hInput, vInput);
        }
	}

    public void OnDead() {
        
    }

    private int legPosOffset = 1;

    private void HandleMovement(float hInput, float vInput) {
        //Debug.Log("OnGround = " + OnGround);
        // If we aren't moving up or down, we're on the ground.
        if (MyRigidbody.velocity.y > 0 && !this.OnGround) {
            //Debug.Log("Trigger Land Animation");
            // land = true triggers landing animation
            mAnimator.SetBool("land", true);
        }

        // If we aren't attacking, or sliding, and are either on the ground or in the air, we move.
        if (!Attack && (OnGround || airControl)) {
            if(isPoisoned)
            {
                var forward = hInput * movementSpeed * -1;
                MyRigidbody.velocity = new Vector3(forward, MyRigidbody.velocity.y, 0);
            }
            else{
                var forward = hInput * movementSpeed;
                MyRigidbody.velocity = new Vector3(forward, MyRigidbody.velocity.y, 0);
            }
        }

        // If we want to jump and we aren't currently falling, add jump force
        if (Jump && !mAnimator.GetBool("land")) {
            MyRigidbody.AddForce(new Vector3(0, jumpForce, 0));
            Jump = false;
        }

        mAnimator.SetFloat("speed", Mathf.Abs(hInput));
        if (MyRigidbody.gravityScale == 0)
        {
            var forward = vInput * movementSpeed;
            MyRigidbody.velocity = new Vector3(MyRigidbody.velocity.x, forward, 0);
        }
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
    
    private bool IsGrounded2D()
    {
        foreach (Transform point in groundPoints)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, groundRadius, whatIsGround);

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
                    return true;
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

    public override void ThrowKnife(int value) {
        if (!OnGround && value == 1 || OnGround && value == 0) {
            base.ThrowKnife(value);
        }
        ThrowNoamPlayer.Play();

    }

    public override IEnumerator TakeDamage(int force = 10) {
        if (!immortal) {
            healthStat.CurrentValue -= force;

            if (!IsDead) {
                mAnimator.SetTrigger("damage");
                Hurt.Play();
                immortal = true;
                StartCoroutine(IndicateImmortal());
                yield return new WaitForSeconds(immortalDuration);

                immortal = false;

            }
            else {
                Death();
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

    private void Death(bool disappear = false)
    {
        mAnimator.SetLayerWeight(1, 0);
        mAnimator.SetTrigger("death");
        BackMusic.Stop();
        DeadSound.Play();
        immortal = true;
        if (disappear)
        {
            SetToInvisible();
        }
    }

    public override void AfterDeath() {
        mAnimator.SetTrigger("idle");
        healthStat.CurrentValue = healthStat.MaxValue;
        transform.position = startPos;

        MyRigidbody.velocity = Vector3.zero;
        this.transform.eulerAngles = new Vector3(0, 0, 0);

        JumpSound.Play();
        immortal = false;
        SetToVisible();
        NormalSize();
        foreach (GameObject gameObject in Garbage)
        {
            gameObject.SetActive(true);
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        // Debug.Log("OnTriggerExit2D");
        if (other.gameObject.tag == "Bath")
        {
            Water.Stop();
        }
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        string tag = other.gameObject.tag;
        Debug.Log("OnTriggerEnter2D with: "+tag);
        base.OnTriggerEnter2D(other);
        if (tag == "MediumCoin")
        {
            GameManager.Instance.CollectedCoins+=25;
            Destroy(other.gameObject);
            CollectibleSound.Play();
        }
        else if (tag == "BathCollider")
        {
            Bath.sortingOrder = 0;
        }
        else if (tag == "Rapetisser")
        {
            WillRapetisser();
            AddToGarbage(other.gameObject);
            CollectibleSound.Play();
        }
        else if (tag == "Grow")
        {
            WillGrow();
            AddToGarbage(other.gameObject);
        }
        else if (tag == "Bath")
        {
            Splash.Play();
            Water.Play();
            foreach (Animator BathAnimator in BathAnimators)
            {
                BathAnimator.SetTrigger("splash");
            }
        }
        else if (tag == "PipeStart")
        {
            MyRigidbody.gravityScale = MyRigidbody.gravityScale == 0 ? 1 : 0;
            mAnimator.SetBool("pipe", mAnimator.GetBool("pipe") ? false : true);
            //ToggleVisibility();
        }
        else if (tag == "PipeEnd")
        {
            MyRigidbody.gravityScale = 1;
            mAnimator.SetBool("pipe", false);
            //SetToVisible();
        }
        else if (tag == "GravityUp")
        {
            MyRigidbody.gravityScale = 1;
        }
        else if (tag == "GravityDown")
        {
            MyRigidbody.gravityScale = 0;
        }
        else if (tag == "SmallCoin")
        {
            GameManager.Instance.CollectedCoins+=10;
            Destroy(other.gameObject);
            CollectibleSound.Play();
        }
        else if (tag == "Poison")
        {
            healthStat.CurrentValue -= 10;
            Destroy(other.gameObject);
            isPoisoned = true;
            movementSpeed = movementSpeed/2;
            mSpriteRenderer.material.color = Color.green;
            Invoke("CureOfPoison", 10);
        }
        else if (tag == "LifePotion")
        {
            LifePotion.Play();
            healthStat.CurrentValue += 10;
            AddToGarbage(other.gameObject);
            CureOfPoison();
        }
        else if (tag == "EndCyrilEtudes")
        {
            HasWonLevel();
            StartCoroutine(ChangeScene("CyrilForet"));
        }
        else if (tag=="EndCyrilForest")
        {
            HasWonLevel();
            StartCoroutine(ChangeScene("CyrilNantes"));
        }
        else if (tag == "EndCyrilTown")
        {
            HasWonLevel();
            StartCoroutine(ChangeScene("MainMenu"));
        }
        else if (tag == "EndAnaisPoudlard")
        {
            HasWonLevel();
            StartCoroutine(ChangeScene("AnaisHopital"));
        }
        else if (tag == "EndAnaisHopital")
        {
            HasWonLevel();
            StartCoroutine(ChangeScene("AnaisNantes"));
        }
        else if (tag == "EndAnaisTown")
        {
            HasWonLevel();
            StartCoroutine(ChangeScene("MainMenu"));
        }
        else if (tag == "EndNoamTown")
        {
            HasWonLevel();
            StartCoroutine(ChangeScene("MainMenu"));
        }
        else if (tag == "Death")
        {
            Death(true);
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

    private void AddToGarbage (GameObject gameObject)
    {
        gameObject.SetActive(false);
        Garbage.Add(gameObject);
    }

    private IEnumerator ChangeScene(string LevelName)
    {
        yield return new WaitForSeconds(5f);
        //Start fading
        float fadeTime = GameObject.Find("FadeScreen").GetComponent<Fading>().BeginFade(1);
        yield return new WaitForSeconds(fadeTime);
        mAnimator.SetBool("win", false);
        immortal = false;
        SceneManager.LoadScene(LevelName);
    }

    private void HasWonLevel(){
        mAnimator.SetBool("win", true);
        win = true; 
        immortal = true;
        BackMusic.Stop();
        WinLevel.Play();
    }

}


