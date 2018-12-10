using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    float speed = 0.01f;
    Rigidbody ball;
	SphereCollider ballCollider;
	Vector3 direction;
    GameObject hitWall, playerWall, leftWall, rightWall;
    LayerMask mask;

    void Start()
    {
        ball = GetComponent<Rigidbody>();
		direction = -Vector3.forward;

        hitWall = GameObject.Find("HitWall");
        playerWall = GameObject.Find("PlayerWall");
        leftWall = GameObject.Find("LeftWall");
        rightWall = GameObject.Find("RightWall");

		mask = LayerMask.GetMask("Walls");
    }

    void FixedUpdate()
    {
		ball.MovePosition(ball.position + transform.parent.TransformDirection(direction * speed));
    }

    private void OnCollisionEnter(Collision col)
    {
    }
}
