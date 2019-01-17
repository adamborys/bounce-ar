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
            GameObject.Find("InGameCanvas").SetActive(false);
            isOpponentReady = value;
        }
    }
    private static bool isOpponentReady;
    public Slider ScaleSlider;
    public Button ReadyButton;
	Vector3 initialScale;
	Vector3 clientRotation = new Vector3(0, 180, 0);
    
    void Start()
    {
        ScaleSlider.onValueChanged.AddListener(delegate { ScaleValueChanged(); });
        ReadyButton.onClick.AddListener(delegate { ReadyClick(); });

        initialScale = new Vector3(5f, 5f, 5f);
        ScaleSlider.value = 0;
        transform.localScale = initialScale;
    }
    // Manipulating Arena transform due to non-parenting it to UserTarget
    void Update()
    {
        transform.position = UserTargetController.UserTargetTransform.position;
        transform.rotation = UserTargetController.UserTargetTransform.rotation;
        if(!NetworkController.IsServer)
            transform.Rotate(clientRotation);
    }

    // Scaling before playing for personal convenience
    private void ScaleValueChanged()
    {
		float scale = Mathf.Pow(ScaleSlider.value + 1f, 2);
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

    private void AnnounceWinner(string winner)
    {
        GameObject.Find("InGameCanvas").SetActive(true);
        GameObject.Find("InGameCanvas").GetComponentInChildren<Text>().text = winner + " won the game!";
    }
}
