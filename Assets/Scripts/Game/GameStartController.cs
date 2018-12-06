using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class GameStartController : MonoBehaviour
{
    public static bool IsReady;
    public bool IsGameOnlyDebugged;
    GameObject debugLight, debugCamera, userTarget;
    Slider scaleSlider;
    Button readyButton;
	Vector3 initialScale;
    // Use this for initialization
    void Start()
    {
        debugLight = GameObject.Find("DebugLight");
        debugCamera = GameObject.Find("DebugCamera");
        if(IsGameOnlyDebugged)
        {

        }
        else
        {
            debugLight.SetActive(false);
            debugCamera.SetActive(false);

            Destroy(GameObject.Find("TestCube"));
            userTarget = GameObject.Find("UserTarget");

            scaleSlider = GameObject.Find("ScaleSlider").GetComponent<Slider>();
            scaleSlider.onValueChanged.AddListener(delegate { ScaleValueChanged(); });
            scaleSlider.value = 0;
            readyButton = GameObject.Find("ReadyButton").GetComponent<Button>();
            readyButton.onClick.AddListener(delegate { ReadyClick(); });

            debugLight.transform.SetParent(transform, false);
            transform.SetParent(userTarget.transform, false);

            initialScale = new Vector3(10f, 10f, 10f);
            transform.localScale = initialScale;
            transform.Translate(new Vector3(0, 0, 0.4f));
        }
    }

    private void ScaleValueChanged()
    {
		float scale = Mathf.Pow(scaleSlider.value + 1f, 2);
        transform.localScale = scale * initialScale;
    }

    private void ReadyClick()
    {
        throw new NotImplementedException();
    }
}
