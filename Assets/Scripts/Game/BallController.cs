using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
	public Vector3 Direction;
    public float Speed;
    private bool isLaunched;
    private SphereCollider ballCollider;
    private Vector3 collisionNormal;
    private LayerMask mask;

    void Start()
    {
        ballCollider = GetComponent<SphereCollider>();
    }

    void FixedUpdate()
    {
        if(isLaunched)
        {
            transform.Translate(Direction * Speed);
        }
    }

    public void LaunchBall()
    {
        isLaunched = true;
    }

    // Scaling before playing for personal convenience
    private void OnCollisionEnter(Collision col)
    {
        // Simple bouncing
        collisionNormal = transform.InverseTransformDirection(col.contacts[0].normal);
        collisionNormal.y = 0;
        collisionNormal.Normalize();
        Direction = Vector3.Reflect(Direction, collisionNormal);
    }
    
    // Fail-safe collider interlock fix
    public void OnCollisionStay(Collision col)
    {
        float depth;
        Vector3 direction;
        Physics.ComputePenetration(ballCollider, transform.position, transform.rotation,
                                    col.collider, col.transform.position, col.transform.rotation,
                                    out direction, out depth);
        direction.y = 0;
        transform.Translate(depth * direction * 1.415f);
    }
}
