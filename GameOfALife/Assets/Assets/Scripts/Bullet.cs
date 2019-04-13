using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float speed;
    private Vector2 direction;

    private Rigidbody2D mRigidBody;

    // Use this for initialization
    void Start()
    {
        mRigidBody = GetComponent<Rigidbody2D>();

    }

    public void Initialize(Vector2 direction)
    {
        this.direction = direction;
    }

    void FixedUpdate()
    {
        mRigidBody.velocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="Wall")
        {
            Destroy(gameObject);
        }
    }
}
