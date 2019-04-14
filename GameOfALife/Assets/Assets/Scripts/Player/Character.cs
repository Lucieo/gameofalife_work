using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Character : MonoBehaviour {

    [SerializeField]
    protected float movementSpeed;

    [SerializeField]
    protected GameObject knifePrefab;
    [SerializeField]
    protected Transform knifeSpawn;

    [SerializeField]
    private Collider2D swordCollider;
    [SerializeField]
    private List<string> damageSources;

    private List<string> unDestroyableDamageSources = new List<string>(new string[] {
         "Pics",
         "EnemyBullet",
         "Sword",
         "EnemySword",
         "DangerousLeg",
         "RotatingLeg",
         "Apple"
    });


    protected bool facingRight;

    protected SpriteRenderer mSpriteRenderer { get; set; }
    public Animator mAnimator { get; private set; }
    public bool Attack { get; set; }

    [SerializeField]
    protected int health;
    public abstract bool IsDead { get; }
    public bool TakingDamage { get; set; }

    // Rapetisser / Grow
    protected bool startRapetisser = false;
    protected bool startGrow = false;
    public float maxSize = 0.6f;
    public float minSize = 0.2f;
    protected float durationInv = 0f;
    public float duration = 5f;
    protected float timer = 0f;

    public int force = 10;


    // Use this for initialization
    public virtual void Start() {
        mAnimator = GetComponent<Animator>();
        facingRight = true;
        mSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public abstract IEnumerator TakeDamage(int damage);
    public abstract void AfterDeath();

    public void ChangeDirection() {
        facingRight = !facingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    public virtual void ThrowKnife(int value) {
        if (facingRight) {
            GameObject temp = (GameObject)Instantiate(knifePrefab, knifeSpawn.position, Quaternion.Euler(new Vector3(0, 0, -90)));
            temp.GetComponent<Knife>().Initialize(Vector2.right);
        }
        else {
            GameObject temp = (GameObject)Instantiate(knifePrefab, knifeSpawn.position, Quaternion.Euler(new Vector3(0, 0, +90)));
            temp.GetComponent<Knife>().Initialize(Vector2.left);
        }
    }

    public void MeleeAttack() {
        swordCollider.enabled = true;
    }

    public void DisableSword()
    {
        swordCollider.enabled = false;
    }

    public virtual void OnTriggerEnter2D(Collider2D other) {
        if (damageSources.Contains(other.tag)) {
            Character character = other.gameObject.GetComponent<Character>();
            Character parentCharacter = other.gameObject.GetComponentInParent<Character>();
            StartCoroutine(TakeDamage(character != null ? character.force : parentCharacter != null ? parentCharacter.force : 10));
            if (!unDestroyableDamageSources.Contains(other.tag))
            {
                Destroy(other.gameObject);
            }
        }
    }

    protected void Rapetisser ()
    {
        if (startRapetisser)
        {
            Debug.Log("Rapetisser");
            float scale = Mathf.Lerp(maxSize, minSize, timer * durationInv);
            timer += Time.deltaTime;
            this.transform.localScale = new Vector3(facingRight ? scale : -scale, scale, 0f);
            if (scale == minSize)
            {
                startRapetisser = false;
            }
        }
    }

    protected void WillRapetisser ()
    {
        // Debug.Log("WillRapetisser: " + startRapetisser + " " + this.transform.localScale.x);
        if (Mathf.Abs(this.transform.localScale.x) > minSize && !startRapetisser)
        {
            // Init, when the fading start, called once
            // Division is the devil, so I prepare the inverse of the duration
            durationInv = 1f / (duration != 0f ? duration : 1f);
            // timer that will go from 0 to duration over time
            timer = 0f;
            startRapetisser = true;
        }
    }

    protected void NormalSize()
    {
        this.transform.localScale = new Vector3(facingRight ? maxSize : -maxSize, maxSize, 0f);
    }

    protected void Grow()
    {
        if (startGrow)
        {
            Debug.Log("Grow");
            float scale = Mathf.Lerp(minSize, maxSize, timer * durationInv);
            timer += Time.deltaTime;
            this.transform.localScale = new Vector3(facingRight ? scale : -scale, scale, 0f);
            if (scale == maxSize)
            {
                startGrow = false;
            }
        }
    }

    protected void WillGrow()
    {
        if (Mathf.Abs(this.transform.localScale.x) < maxSize && !startGrow)
        {
            Debug.Log("WillGrow");
            // Init, when the fading start, called once
            // Division is the devil, so I prepare the inverse of the duration
            durationInv = 1f / (duration != 0f ? duration : 1f);
            // timer that will go from 0 to duration over time
            timer = 0f;
            startGrow = true;
        }
    }

    protected void SetToInvisible()
    {
        mSpriteRenderer.color = Color.clear;
    }

    protected void SetToVisible()
    {
        mSpriteRenderer.color = Color.white;
    }

    protected void ToggleVisibility()
    {
        if (mSpriteRenderer.color == Color.clear)
        {
            SetToVisible();
        }
        else
        {
            SetToInvisible();
        }
    }
}
