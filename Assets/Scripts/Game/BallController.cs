using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
	public Vector3 Direction;
    public float Speed;
    bool isLaunched;
    Rigidbody ball;
	SphereCollider ballCollider;
    LayerMask mask;

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

    private void OnCollisionEnter(Collision col)
    {
        // Simple bouncing
        Vector3 colliderNormal = transform.InverseTransformDirection(col.contacts[0].normal);
        Direction = Vector3.Reflect(Direction, colliderNormal);
    }
}
