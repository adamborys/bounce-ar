using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private Slider horizontalSlider, verticalSlider;
    private Button calibrateButton, buildButton;
    private Text testLog;
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

        testLog = GameObject.Find("TestLog").GetComponent<Text>();
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
        throw new NotImplementedException();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalSlider.value = (float)(acceleration.x / 2 + 0.5);
        verticalSlider.value = (float)(acceleration.y / 2 + 0.5);

        testLog.text = acceleration.x.ToString("0.00");
        testLog.text = acceleration.y.ToString("0.00");
    }
}
