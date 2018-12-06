using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {
	float speed = 0.01f;
	Vector3 direction = Vector3.forward;
	void FixedUpdate () 
	{
		transform.Translate(direction * speed);
	}
}
