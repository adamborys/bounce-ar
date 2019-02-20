using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public static Vector2 ServerStartVelocity = new Vector2(0,10);
    public static Vector2 ClientStartVelocity = new Vector2(0,-10);
    public Rigidbody2D ballBody; 
    public float speed = 10;
    private SphereCollider ballCollider;

    void Awake()
    {
        ballCollider = GetComponent<SphereCollider>();
        ballBody = GetComponent<Rigidbody2D>();
    }

    public void FreezeBall()
    {
        ballBody.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void NormalizeBallVelocity()
    {
        ballBody.velocity = ballBody.velocity.normalized * speed;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        //if(NetworkController.IsServer)
        {
            GetComponentInParent<GameServerController>()
                .OnBallCollision(gameObject.name, col.gameObject.name);
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        NormalizeBallVelocity();
    }
}
