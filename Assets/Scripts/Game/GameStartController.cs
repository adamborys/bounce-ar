using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class GameStartController : MonoBehaviour
{
    public static bool IsReady;
    Slider scaleSlider;
    Button readyButton;
	Vector3 initialScale;
    // Use this for initialization
    void Start()
    {
        Destroy(GameObject.Find("TestCube"));

        scaleSlider = GameObject.Find("ScaleSlider").GetComponent<Slider>();
        scaleSlider.onValueChanged.AddListener(delegate { ScaleValueChanged(); });
        scaleSlider.value = 0;
        readyButton = GameObject.Find("ReadyButton").GetComponent<Button>();
        readyButton.onClick.AddListener(delegate { ReadyClick(); });

        initialScale = new Vector3(10f, 10f, 10f);
        transform.localScale = initialScale;
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
