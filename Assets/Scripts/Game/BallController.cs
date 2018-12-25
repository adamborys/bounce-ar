using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
	public Vector3 direction;
    public float speed = 0.1f;
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
            transform.Translate(direction * speed);
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
        direction = Vector3.Reflect(direction, colliderNormal);
    }
}
