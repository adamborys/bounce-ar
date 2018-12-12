using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class ArenaController : MonoBehaviour
{
    public Slider scaleSlider;
    public Button readyButton;
	Vector3 initialScale;
    // Use this for initialization
    void Start()
    {
        Destroy(GameObject.Find("TestCube"));

        scaleSlider.onValueChanged.AddListener(delegate { ScaleValueChanged(); });
        readyButton.onClick.AddListener(delegate { ReadyClick(); });

        initialScale = new Vector3(10f, 10f, 10f);
        scaleSlider.value = 0;
        transform.localScale = initialScale;
    }

    void Update()
    {
        transform.position = UserTargetController.UserTargetTransform.position;
        transform.rotation = UserTargetController.UserTargetTransform.rotation;
    }

    private void ScaleValueChanged()
    {
		float scale = Mathf.Pow(scaleSlider.value + 1f, 2);
        transform.localScale = scale * initialScale;
    }

    private void ReadyClick()
    {
        transform.GetChild(0).gameObject.GetComponent<BallController>().LaunchBall();
        Destroy(gameObject.scene.GetRootGameObjects()[1]);

    }
}
