using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TargetBuilderUIController : MonoBehaviour
{
    public Button CalibrateButton, BuildButton, StartButton;
    public Slider horizontalSlider, verticalSlider;
    private Quaternion calibration;
    private Vector3 acceleration
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

        CalibrateButton.onClick.AddListener(delegate { CalibrateClick(); });
        BuildButton.onClick.AddListener(delegate { BuildClick(); });
        StartButton.onClick.AddListener(delegate { StartClick(); });
    }

    // Setting calibration quaternion
    private void CalibrateClick()
    {
        Vector3 accelerationSnapshot = Input.acceleration;
        Quaternion fixedSnapshot =
            Quaternion.FromToRotation(new Vector3(0.0f, 0.0f, -1.0f), accelerationSnapshot);
        calibration = Quaternion.Inverse(fixedSnapshot);
    }
    // Building new UserTarget
    private void BuildClick()
    {
        UserTargetController itc = GameObject.Find("UserTarget").GetComponent<UserTargetController>();
        itc.BuildTarget();
    }

    private void StartClick()
    {
        StartCoroutine(loadGameSceneAsync());
    }

    // Displaying changes of device's rotation according to accelerometer data and
    // deciding whether user should be able to build target at current arrangement
    void Update()
    {
        horizontalSlider.value = (float)(acceleration.x / 2 + 0.5);
        verticalSlider.value = (float)(acceleration.y / 2 + 0.5);
        if (0.45 <= horizontalSlider.value && horizontalSlider.value <= 0.55 &&
            0.45 <= verticalSlider.value && verticalSlider.value <= 0.55 &&
            !UserGuideUIController.IsActive)
        {
            BuildButton.interactable = true;
        }
        else
        {
            BuildButton.interactable = false;
        }
    }
    // Loading game scene asynchronously
    // Destroying test cube, UI and preserving Network Provider
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
