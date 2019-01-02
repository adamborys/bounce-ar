using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
	public Vector3 Direction;
    public float Speed;
    private bool isLaunched;
    private Rigidbody ball;
    private LayerMask mask;

    void Start()
    {
        ball = GetComponent<Rigidbody>();
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
        Vector3 colliderNormal = transform.InverseTransformDirection(col.contacts[0].normal);
        Direction = Vector3.Reflect(Direction, colliderNormal);
        Direction.y = 0;
    }
}
