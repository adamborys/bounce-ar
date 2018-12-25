using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TargetBuilderUIController : MonoBehaviour
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
        horizontalSlider = transform.GetChild(0).GetComponent<Slider>();
        verticalSlider = transform.GetChild(1).GetComponent<Slider>();

        calibrateButton = transform.GetChild(2).GetComponent<Button>();
        calibrateButton.onClick.AddListener(delegate { CalibrateClick(); });
        buildButton = transform.GetChild(3).GetComponent<Button>();
        buildButton.onClick.AddListener(delegate { BuildClick(); });
        startButton = transform.GetChild(4).GetComponent<Button>();
        startButton.onClick.AddListener(delegate { StartClick(); });
    }

    private void CalibrateClick()
    {
        // Setting (calibrating) displayed accelerometer values to neutral (0.5)
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
        StartCoroutine(loadGameSceneAsync());
    }

    void Update()
    {
        // Displaying changes of device's rotation according to accelerometer data and
        // deciding whether user should be able to build target at current arrangement
        horizontalSlider.value = (float)(acceleration.x / 2 + 0.5);
        verticalSlider.value = (float)(acceleration.y / 2 + 0.5);
        if (0.45 <= horizontalSlider.value && horizontalSlider.value <= 0.55 &&
            0.45 <= verticalSlider.value && verticalSlider.value <= 0.55 &&
            !UserGuideUIController.IsActive)
        {
            buildButton.interactable = true;
        }
        else
        {
            buildButton.interactable = false;
        }
    }

    private IEnumerator loadGameSceneAsync()
    {
        Destroy(UserTargetController.UserTargetTransform.GetChild(0).gameObject);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);

        while(!asyncLoad.isDone)
        {
            yield return null;
        }
        if(NetworkController.IsServer)
            ServerController.GameController = GameObject.Find("Arena").AddComponent<GameServerController>();
        else
            ClientController.GameController = GameObject.Find("Arena").AddComponent<GameClientController>();
        Destroy(gameObject); // Destroy TargetBuilder UI
    }
}
