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
        ballCollider = GetComponent<SphereCollider>();
		ball.isKinematic = true;
		direction = new Vector3(0, 0, -1);

        hitWall = GameObject.Find("HitWall");
        playerWall = GameObject.Find("PlayerWall");
        leftWall = GameObject.Find("LeftWall");
        rightWall = GameObject.Find("RightWall");

		mask = LayerMask.GetMask("Walls");
    }

    void FixedUpdate()
    {
		transform.Translate(direction * speed);
    }

    private void OnTriggerEnter(Collider hitCollider)
    {
		// Raycast to figure out the normal of the trigger intersection
		RaycastHit hitInfo = new RaycastHit();
		Vector3 direction = hitCollider.bounds.center - ballCollider.bounds.center;
		Debug.Log(direction.x);
		Debug.Log(direction.y);
		Debug.Log(direction.z);
		Physics.Raycast(ballCollider.bounds.center, direction, out hitInfo, Mathf.Infinity, mask);
		
		// Now we can calculate the bounce direction
		direction = Vector3.Reflect(direction, hitInfo.normal);
    }
}
