using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class GameController : MonoBehaviour 
{
	GameObject userTarget;
	// Use this for initialization
	void Start () 
	{
		Destroy(GameObject.Find("TestCube"));
		userTarget = GameObject.Find("UserTarget");
		transform.SetParent(userTarget.transform, false);
	}
}
