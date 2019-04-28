using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float speed;
    private Vector2 direction;
    [SerializeField] string facing;

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
        if(facing=="right"){
            mRigidBody.velocity = Vector2.right * speed;
        }
        else{
            mRigidBody.velocity = Vector2.left * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="Wall")
        {
            Destroy(gameObject);
        }
    }
}
