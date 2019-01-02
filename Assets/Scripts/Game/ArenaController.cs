using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class ArenaController : MonoBehaviour
{
    public static bool IsReady;
    public static bool IsOpponentReady
    {
        get
        {
            return isOpponentReady;
        }
        set
        {
            Destroy(GameObject.Find("NotReadyCanvas"));
            isOpponentReady = value;
        }
    }
    private static bool isOpponentReady;
    public Slider scaleSlider;
    public Button readyButton;
	Vector3 initialScale;
    
    void Start()
    {
        scaleSlider.onValueChanged.AddListener(delegate { ScaleValueChanged(); });
        readyButton.onClick.AddListener(delegate { ReadyClick(); });

        initialScale = new Vector3(5f, 5f, 5f);
        scaleSlider.value = 0;
        transform.localScale = initialScale;
    }
    // Manipulating Arena transform due to non-parenting it to UserTarget
    void Update()
    {
        transform.position = UserTargetController.UserTargetTransform.position;
        transform.rotation = UserTargetController.UserTargetTransform.rotation;
    }

    // Scaling before playing for personal convenience
    private void ScaleValueChanged()
    {
		float scale = Mathf.Pow(scaleSlider.value + 1f, 2);
        transform.localScale = scale * initialScale;
    }
    // Destroying UI after Readiness reported
    private void ReadyClick()
    {
        // Destroying game start UI
        Destroy(GameObject.Find("GameStartUICanvas"));
        IsReady = true;
        if (NetworkController.IsServer)
        {
            StartCoroutine(NetworkController.Provider
                                            .GetComponent<ServerController>().ReadinessTransmitter);
        }
        else
        {
            StartCoroutine(NetworkController.Provider
                                            .GetComponent<ClientController>().ReadinessTransmitter);
        }
    }
}
