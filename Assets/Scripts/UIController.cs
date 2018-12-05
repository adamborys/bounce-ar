using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static Button calibrateButton, buildButton, startButton;
    private Slider horizontalSlider, verticalSlider;
    Quaternion calibration;
    Vector3 acceleration
    {
        get
        {
            return calibration * Input.acceleration;
        }
    }

    // Use this for initialization
    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        horizontalSlider = GameObject.Find("HorizontalSlider").GetComponent<Slider>();
        verticalSlider = GameObject.Find("VerticalSlider").GetComponent<Slider>();

        calibrateButton = GameObject.Find("CalibrateButton").GetComponent<Button>();
        calibrateButton.onClick.AddListener(delegate { CalibrateClick(); });
        buildButton = GameObject.Find("BuildButton").GetComponent<Button>();
        buildButton.onClick.AddListener(delegate { BuildClick(); });
        startButton = GameObject.Find("StartButton").GetComponent<Button>();
        startButton.onClick.AddListener(delegate { StartClick(); });
    }

    private void CalibrateClick()
    {
        Vector3 accelerationSnapshot = Input.acceleration;
        Quaternion fixedSnapshot = 
			Quaternion.FromToRotation(new Vector3(0.0f, 0.0f, -1.0f), accelerationSnapshot);
        calibration = Quaternion.Inverse(fixedSnapshot);
    }
    private void BuildClick()
    {
        UserTargetController itc = GameObject.Find("UserTarget").GetComponent<UserTargetController>();
		itc.BuildTarget();
    }

    private void StartClick()
    {
        throw new NotImplementedException();
    }

    void Update()
    {
        horizontalSlider.value = (float)(acceleration.x / 2 + 0.5);
        verticalSlider.value = (float)(acceleration.y / 2 + 0.5);
		if(0.45 <= horizontalSlider.value && horizontalSlider.value <= 0.55 &&
			0.45 <= verticalSlider.value && verticalSlider.value <= 0.55)
		{
			buildButton.interactable = true;
		}
		else
		{
			buildButton.interactable = false;
		}
    }
}
