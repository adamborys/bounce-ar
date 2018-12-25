using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class ArenaController : MonoBehaviour
{
    public static bool IsReady;
    public Slider scaleSlider;
    public Button readyButton;
	Vector3 initialScale;
    // Use this for initialization
    void Start()
    {
        scaleSlider.onValueChanged.AddListener(delegate { ScaleValueChanged(); });
        readyButton.onClick.AddListener(delegate { ReadyClick(); });

        initialScale = new Vector3(10f, 10f, 10f);
        scaleSlider.value = 0;
        transform.localScale = initialScale;
    }

    void Update()
    {
        // Manipulating Arena transform due to non-parenting it to UserTarget
        transform.position = UserTargetController.UserTargetTransform.position;
        transform.rotation = UserTargetController.UserTargetTransform.rotation;
    }

    private void ScaleValueChanged()
    {
        // Scaling before playing for personal convenience
		float scale = Mathf.Pow(scaleSlider.value + 1f, 2);
        transform.localScale = scale * initialScale;
    }

    private void ReadyClick()
    {
        // Destroying game start UI
        Destroy(gameObject.scene.GetRootGameObjects()[1]);
        IsReady = true;
    }
}
