using UnityEngine;
using System.Collections;

public class Enemy : Character {

    enum DropItem
    {
        Coin = 0,
        Rapetisser = 1,
    }
    private IEnemyState currentState;
    public GameObject Target { get; set; }

    [SerializeField]
    private float meleeRange;
    [SerializeField]
    private float throwRange;

    private bool dumb;

    [SerializeField]
    private Vector3 startPos;

    [SerializeField]private Transform leftEdge;
    [SerializeField]private Transform rightEdge;

    private bool shouldDropItem = true;
    [SerializeField]
    private DropItem dropItem;

    public bool InMeleeRange {
        get {
            if (Target != null) {
                return Vector2.Distance(transform.position, Target.transform.position) <= meleeRange;
            }

            return false;
        }
    }

    public bool InThrowRange {
        get {
            if (Target != null) {
                return Vector2.Distance(transform.position, Target.transform.position) <= throwRange;
            }

            return false;
        }
    }

	// Use this for initialization
	public override void Start () {
        base.Start();
        ChangeDirection();
        Player.Instance.Dead += new DeadEventHandler(RemoveTarget);
        ChangeState(new IdleState());
    }
	
	// Update is called once per frame
	void Update () {
        if (!IsDead) {
            if (!TakingDamage) {
                currentState.Execute();
            }
            LookAtTarget();
        }

        //////////////////////////////
        // DUMBEST SHIT EVER UNITY PLS
        //////////////////////////////
        if (dumb) {
            transform.Translate(new Vector3(0.0001f, 0, 0));
        }
        else {
            transform.Translate(new Vector3(-0.0001f, 0, 0));
        }
        dumb = !dumb;
	}

    public void ChangeState(IEnemyState newState) {
        if (currentState != null) {
            currentState.Exit();
        }

        currentState = newState;
        currentState.Enter(this);
    }

    public void Move()
    {
        if (!Attack)
        {
            if (GetDirection().x > 0 && transform.position.x < rightEdge.position.x || GetDirection().x < 0 && transform.position.x > leftEdge.position.x)
            {
                mAnimator.SetFloat("speed", 1);
                transform.Translate(GetDirection() * movementSpeed * Time.deltaTime);
            }
            else if (currentState is PatrolState)
            {
                ChangeDirection();
            }
            else if(currentState is RangedState)
            {
                ChangeState(new IdleState());
                Target = null;
            }
        }
    }


    private void LookAtTarget() {
        if (Target != null) {
            float xDir = Target.transform.position.x - transform.position.x;
            if (xDir < 0 && facingRight || xDir > 0 && !facingRight) {
                ChangeDirection();
            }
        }
    }

    public void RemoveTarget() {
        Target = null;
        ChangeState(new PatrolState());
    }

    public Vector2 GetDirection() {
        return facingRight ? Vector2.right : Vector2.left;
    }

    public override void OnTriggerEnter2D(Collider2D other) {
        base.OnTriggerEnter2D(other);
        currentState.OnTriggerEnter(other);
    }


    public override IEnumerator TakeDamage(int force = 10) {
        health -= force;
        if (!IsDead) {
            mAnimator.SetTrigger("damage");
        }
        else {
            mAnimator.SetTrigger("death");
            Destroy(GetComponent<Rigidbody2D>());
            GetComponent<BoxCollider2D>().enabled = false;
            if(shouldDropItem)
            {
                Debug.Log("shouldDropItem " + dropItem);
                GameObject item = (GameObject) Instantiate(
                    dropItem == DropItem.Rapetisser ? GameManager.Instance.RapetisserPrefab : GameManager.Instance.CoinPrefab, 
                    new Vector3(transform.position.x, transform.position.y + 2), 
                    Quaternion.identity
                );
                Debug.Log(item.transform.name);
                Physics2D.IgnoreCollision(item.GetComponent<Collider2D>(), GetComponent<Collider2D>());
                shouldDropItem = false;
            }
            
        }
        yield return null;
    }

    public override bool IsDead {
        get {
            return health <= 0;
        }
    }

    public override void AfterDeath() {
        Destroy(gameObject);
        //mAnimator.ResetTrigger("death");
        //mAnimator.SetTrigger("idle");
    }
}
