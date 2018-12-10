using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public float speed = 0.1f;
    bool isLaunched;
    Rigidbody ball;
	SphereCollider ballCollider;
	Vector3 direction;
    GameObject hitWall, playerWall, leftWall, rightWall;
    LayerMask mask;

    void Start()
    {
        ball = GetComponent<Rigidbody>();

        hitWall = GameObject.Find("HitWall");
        playerWall = GameObject.Find("PlayerWall");
        leftWall = GameObject.Find("LeftWall");
        rightWall = GameObject.Find("RightWall");

		mask = LayerMask.GetMask("Walls");
        direction = -Vector3.forward;
    }

    void FixedUpdate()
    {
        if(isLaunched)
        {
            // Poruszanie na sztywno, bo ruch kamery odkształca ruch sterowany zmianą ball.velocity
            transform.Translate(direction * speed);
        }
    }

    public void LaunchBall()
    {
        StartCoroutine(launchBall());
    }

    private void OnCollisionEnter(Collision col)
    {
        Debug.Log(direction);
        Vector3 colliderNormal = transform.InverseTransformDirection(col.contacts[0].normal);
        direction = Vector3.Reflect(direction, colliderNormal);
        Debug.Log(direction);
    }

    private IEnumerator launchBall()
    {
        yield return new WaitForSeconds(3);
        isLaunched = true;
    }
}
