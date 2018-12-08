using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {
	float speed = 10f;
	Vector3 velocity;
	Rigidbody ball;
	GameObject hitWall, playerWall, leftWall, rightWall;

	void Start()
	{
		ball = GetComponent<Rigidbody>();
		velocity = ball.velocity = new Vector3(0,0,-1) * speed;

		hitWall = GameObject.Find("HitWall");
		playerWall = GameObject.Find("PlayerWall");
		leftWall = GameObject.Find("LeftWall");
		rightWall = GameObject.Find("RightWall");
	}

    void OnCollisionEnter (Collision col)
    {
		if(col.gameObject == playerWall)
		{
			ball.isKinematic = true;
		}
		else
		{
			velocity = ball.velocity = Vector3.Reflect(velocity, col.contacts[0].normal);
		}
    }

	void OnCollisionExit (Collision col)
	{
		
	}
}
