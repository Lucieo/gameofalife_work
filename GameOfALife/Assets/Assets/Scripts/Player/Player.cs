using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public delegate void DeadEventHandler();

public class Player : Character
{
    private static Player instance;
    public event DeadEventHandler Dead;
    private Fading fading;

    [SerializeField] private Stat healthStat;

    public static Player Instance
    {
        get
        {
            if (instance == null)
            {
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
    public bool Jump { get; set; }
    
    private bool _onGround;
    public bool OnGround { 
        get {
            return _onGround;
        }
        set {
            if (value != _onGround){
                HandleLayers(value);
            }
            _onGround = value;
        }
    }

    private Color originalColor;

    [SerializeField]
    private Vector3 startPos;

    private bool win = false;
    private bool immortal = false;
    [SerializeField]
    private float immortalDuration;

    private bool isPoisoned = false;
    private float poisonedDelay = 5;
    private float poisonedTimer = 0;

    [SerializeField] AudioSource BackMusic;
    [SerializeField] AudioSource JumpSound;
    [SerializeField] AudioSource CollectibleSound;
    [SerializeField] AudioSource DeadSound;
    [SerializeField] AudioSource ThrowEnnemy;
    [SerializeField] AudioSource Hurt;
    [SerializeField] AudioSource CollectCoin;
    [SerializeField] AudioSource ThrowPlayer;
    [SerializeField] AudioSource WinLevel;
    [SerializeField] AudioSource LifePotion;
    [SerializeField] AudioSource PunchSound;
    [SerializeField] AudioSource HealSound;
    [SerializeField] AudioSource DrunkSound;
    [SerializeField] AudioSource PoisonSound;
    [SerializeField] AudioSource Splash;
    [SerializeField] AudioSource Water;

    public bool isBathInScene = false;
    public bool isNoam = false;

    private SpriteRenderer bath;
    private Animator[] bathAnimators;
    private List<GameObject> garbage;

    private Vector2 crouchSize = new Vector2(1.63f, 1.8f);
    private Vector2 crouchOffset = new Vector2(0f, -1.3f);
    private Vector2 colliderInitialSize;
    private Vector2 colliderInitialOffset;

    private bool hasWon = false;

    //Gestion du mode high
    private bool isHigh = false;

    private void Awake()
    {
        healthStat.Initialize();
    }

    public override void Start()
    {
        base.Start();
        MyRigidbody = GetComponent<Rigidbody2D>();
        originalColor = mSpriteRenderer.material.color;
        BackMusic.Play();
        colliderInitialSize = GetComponent<BoxCollider2D>().size;
        colliderInitialOffset = GetComponent<BoxCollider2D>().offset;

        if (isBathInScene)
        {
            bath = GameObject.FindGameObjectWithTag("Bath").GetComponent<SpriteRenderer>();
            bathAnimators = new Animator[2];
            GameObject[] splashes = GameObject.FindGameObjectsWithTag("Splash");
            int i = 0;
            foreach (GameObject splash in splashes)
            {
                bathAnimators[i++] = splash.GetComponent<Animator>();
            }
        }
        
        garbage = new List<GameObject>();
        fading = GameObject.Find("FadeScreen").GetComponent<Fading>();
    }

    void Update()
    {
        if (!TakingDamage && !IsDead)
        {
            /*if (transform.position.y <= -14f)
            {
                Death();
            }*/
            HandleInput();
        }

        // Easeier pausing
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Break();
        }

        //RemoveLife if poisonned
        if (isPoisoned && !IsDead)
        {
            if (poisonedTimer >= poisonedDelay)
            {
                TakeDamage();
                poisonedTimer = 0;
            }
            else
            {
                poisonedTimer += Time.deltaTime;
            }
        }

    }

    void FixedUpdate()
    {
        Rapetisser();
        Grow();
        if (!isBathInScene || (isBathInScene && !mAnimator.GetBool("pipe"))) {
            if (!TakingDamage && !IsDead)
            {
                float hInput = Input.GetAxis("Horizontal");

                OnGround = IsGrounded();

                HandleMovement(hInput, 0);
                Flip(hInput);
            }
        } else {
            float hInput = Input.GetAxis("Horizontal");
            float vInput = Input.GetAxis("Vertical");
            HandleMovement(hInput, vInput);
        }
    }

    public void OnDead()
    {
        if (Dead != null)
        {
            Dead();
        }
    }

    private int legPosOffset = 1;

    private void HandleMovement(float hInput, float vInput)
    {

        // If we aren't attacking, or sliding, and are either on the ground or in the air, we move.
        if (!Attack && (OnGround || airControl) && !hasWon)
        {
            if (isPoisoned)
            {
                var forward = hInput * movementSpeed * -1;
                MyRigidbody.velocity = new Vector3(forward, MyRigidbody.velocity.y, 0);
            }
            else
            {
                MyRigidbody.velocity = new Vector2(hInput * movementSpeed, MyRigidbody.velocity.y);
            }
        }

        // If we want to jump and we aren't currently falling, add jump force
        /*if (Jump && MyRigidbody.velocity.y == 0)
        {
            MyRigidbody.AddForce(new Vector2(0, jumpForce));
        }*/
        if (Jump && !mAnimator.GetBool("land"))
        {
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

    public void Land() {
        mAnimator.SetBool("land", true);
    }

    // Checks inputs
    private void HandleInput()
    {
        //Block all movement when level is won
        if (!hasWon && !IsDead)
        {
            if (Input.GetButtonDown("Fire1") && !hasWon)
            {
                mAnimator.ResetTrigger("attack");
                mAnimator.SetTrigger("attack");
                PunchSound.Play();
            }

            if (Input.GetButtonDown("Jump"))
            {
                mAnimator.SetTrigger("jump");
                JumpSound.Play();
            }
            if (Input.GetButtonDown("Fire2"))
            {
                mAnimator.SetTrigger("throw");
            }
            if (Input.GetKey(KeyCode.DownArrow) && !isNoam)
            {
                mAnimator.SetBool("crouch", true);
                GetComponent<BoxCollider2D>().size = crouchSize;
                GetComponent<BoxCollider2D>().offset = crouchOffset;
            }
            else
            {
                mAnimator.SetBool("crouch", false);
                GetComponent<BoxCollider2D>().size = colliderInitialSize;
                GetComponent<BoxCollider2D>().offset = colliderInitialOffset;
            }
        }
    }

    private void Flip(float hInput)
    {
        if (!mAnimator.GetCurrentAnimatorStateInfo(0).IsTag("attack"))
        {
            if ((hInput > 0 && !facingRight || hInput < 0 && facingRight) && !isPoisoned)
            {
                ChangeDirection();
            }
            else if ((hInput < 0 && !facingRight || hInput > 0 && facingRight) && isPoisoned)
            {
                ChangeDirection();
            }
        }
    }

    private bool IsGrounded()
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

    private void HandleLayers(bool onGround)
    {
        if (!onGround)
        {
            mAnimator.SetLayerWeight(1, 1);
            mAnimator.Play("AnimIdle", 0);
        }

        else
        {
            mAnimator.SetLayerWeight(1, 0);
            mAnimator.Play("DefaultState", 1);
        }
    }

    public void KnifeAnimation(int value)
    {
        ThrowKnife(value);
    }
    public override void ThrowKnife(int value)
    {
        if (!OnGround && value == 1 || OnGround && value == 0)
        {
            base.ThrowKnife(value);
        }
        ThrowPlayer.Play();
    }

    public override IEnumerator TakeDamage(int damage = DEFAULT_FORCE)
    {
        if (!immortal)
        {
            healthStat.CurrentValue -= damage;

            if (!IsDead)
            {
                mAnimator.SetTrigger("damage");
                Hurt.Play();
                immortal = true;
                StartCoroutine(IndicateImmortal());
                yield return new WaitForSeconds(immortalDuration);
                immortal = false;

            }
            else
            {
                Death();
            }
        }
    }

    private IEnumerator IndicateImmortal()
    {
        while (immortal)
        {
            mSpriteRenderer.enabled = false;
            yield return new WaitForSeconds(.1f);
            mSpriteRenderer.enabled = true;
            yield return new WaitForSeconds(.1f);
        }
    }

    public override bool IsDead
    {
        get
        {
            if (healthStat.CurrentValue <= 0)
            {
                OnDead();
            }
            return healthStat.CurrentValue <= 0;
        }
    }

    public void Death(bool disappear = false)
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

    public override void AfterDeath()
    {
        sharedGameManager.Life -= 1;
        if (sharedGameManager.isGameOver)
        {
            StartCoroutine(ChangeScene("End"));
            return;
        }

        MyRigidbody.velocity = Vector2.zero;
        this.transform.eulerAngles = new Vector3(0, 0, 0);
        mAnimator.SetTrigger("idle");
        healthStat.CurrentValue = healthStat.MaxValue;
        transform.position = startPos;
        BackMusic.Play();
        JumpSound.Play();
        immortal = false;
        SetToVisible();
        NormalSize();
        foreach (GameObject gameObject in garbage)
        {
            gameObject.SetActive(true);
        }
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        string tag = other.gameObject.tag;
        base.OnTriggerEnter2D(other);
        if (tag == "MediumCoin")
        {
            sharedGameManager.CollectedCoins += 25;
            Destroy(other.gameObject);
            CollectibleSound.Play();
        }
        else if (tag == "SmallCoin")
        {
            sharedGameManager.CollectedCoins += 10;
            Destroy(other.gameObject);
            CollectibleSound.Play();
        }
        else if (tag == "Poison")
        {
            PoisonSound.Play();
            healthStat.CurrentValue -= 10;
            Destroy(other.gameObject);
            isPoisoned = true;
            movementSpeed = movementSpeed / 2;
            mSpriteRenderer.material.color = Color.green;
            Invoke("CureOfPoison", 10);
        }
        else if (tag == "LifePotion")
        {
            LifePotion.Play();
            healthStat.CurrentValue += 10;
            Destroy(other.gameObject);
            CureOfPoison();
        }
        else if (tag == "HighBaby")
        {
            sharedGameManager.CollectedCoins += 50;
            CollectibleSound.Play();
            Destroy(other.gameObject);
            StartCoroutine(GetHigh());
        }
        else if (tag == "DrunkBaby")
        {
            DrunkSound.Play();
            sharedGameManager.CollectedCoins += 50;
            CollectibleSound.Play();
            Destroy(other.gameObject);
            StartCoroutine(GetDrunk());
        }
        else if (tag == "EndCyrilEtudes")
        {
            HasWonLevel("CyrilForet");
        }
        else if (tag == "EndCyrilForest")
        {
            HasWonLevel("CyrilNantes");
        }
        else if (tag == "EndCyrilTown")
        {
            HasWonLevel("ScoreRecord");
        }
        else if (tag == "EndAnaisPoudlard")
        {
            HasWonLevel("AnaisHopital");
        }
        else if (tag == "EndAnaisHopital")
        {
            HasWonLevel("AnaisNantes");
        }
        else if (tag == "EndAnaisTown")
        {
            HasWonLevel("ScoreRecord");
        }
        // NOAM
        else if (tag == "BathCollider")
        {
            bath.sortingOrder = 0;
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
            foreach (Animator bathAnimator in bathAnimators)
            {
                bathAnimator.SetTrigger("splash");
            }
        }
        else if (tag == "PipeStart")
        {
            MyRigidbody.gravityScale = MyRigidbody.gravityScale == 0 ? 1 : 0;
            mAnimator.SetBool("pipe", mAnimator.GetBool("pipe") ? false : true);
        }
        else if (tag == "PipeEnd")
        {
            MyRigidbody.gravityScale = 1;
            mAnimator.SetBool("pipe", false);
        }
        else if (tag == "GravityUp")
        {
            MyRigidbody.gravityScale = 1;
        }
        else if (tag == "GravityDown")
        {
            MyRigidbody.gravityScale = 0;
        }
        else if (tag == "EndNoamTown")
        {
            HasWonLevel("NoamLit");
        }
        else if (tag == "EndNoamLit")
        {
            HasWonLevel("ScoreRecord");
        }
        else if (tag == "Death")
        {
            Death(true);
        }
    }

    private void CureOfPoison()
    {
        if (isPoisoned)
        {
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
            sharedGameManager.CollectedCoins += 50;
            Destroy(other.gameObject);
            CollectCoin.Play();
        }
    }

    private void AddToGarbage(GameObject gameObject)
    {
        gameObject.SetActive(false);
        garbage.Add(gameObject);
    }

    private IEnumerator ChangeScene(string LevelName)
    {
        yield return new WaitForSeconds(5f);
        //Start fading
        float fadeTime = fading.BeginFade(1);
        yield return new WaitForSeconds(fadeTime);
        mAnimator.SetBool("win", false);
        immortal = false;
        hasWon = false;
        SceneManager.LoadScene(LevelName);
    }

    private void HasWonLevel(string nextLevel)
    {
        if (nextLevel == "End")
        {
            sharedGameManager.Win();
        }

        //set character in animation win
        mAnimator.SetBool("win", true);
        //Block all movements
        hasWon = true;
        MyRigidbody.velocity = Vector3.zero; ;
        //Be immortal
        immortal = true;
        StartCoroutine(IndicateImmortal());
        //Play Music
        BackMusic.Stop();
        WinLevel.Play();
        StartCoroutine(ChangeScene(nextLevel));
    }


    private IEnumerator HighMan()
    {
        while (isHigh)
        {
            mSpriteRenderer.material.color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            yield return new WaitForSeconds(0.4f);
        }
    }

    private IEnumerator GetHigh()
    {
        isHigh = true;
        StartCoroutine(HighMan());
        movementSpeed = movementSpeed / 2;
        yield return new WaitForSeconds(immortalDuration);
        isHigh = false;
        mSpriteRenderer.material.color = originalColor;
        movementSpeed = 2 * movementSpeed;
    }

    private IEnumerator GetDrunk()
    {
        Quaternion originalRot = transform.rotation;
        MyRigidbody.constraints = RigidbodyConstraints2D.None;
        movementSpeed = movementSpeed / 2;
        yield return new WaitForSeconds(immortalDuration);
        transform.rotation = originalRot;
        MyRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        movementSpeed = 2 * movementSpeed;
    }
}


