using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {
	float speed = 10f;
	Vector3 direction = Vector3.forward;
	Rigidbody ball;

	void Start()
	{
		ball = GetComponent<Rigidbody>();
		ball.velocity = direction * speed;
	}

    void OnCollisionEnter (Collision col)
    {
		ball.velocity = -direction * speed;
		// TUTAJ OBSŁUŻYĆ ODBICIE
		switch(col.gameObject.name)
		{
			case "HitWall":
			break;
			
			case "PlayerWall":
			break;
			
			case "LeftWall":
			break;
			
			case "RightWall":
			break;
		}
    }
}
