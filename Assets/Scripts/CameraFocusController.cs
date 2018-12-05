using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class CameraFocusController : MonoBehaviour
{
    void Start()
    {
        var vuforiaController = VuforiaARController.Instance;
        vuforiaController.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        vuforiaController.RegisterOnPauseCallback(OnPaused);
    }

    private void OnPaused(bool paused)
    {
        if (!paused) // resumed
        {
            // Set again autofocus mode when app is resumed
            CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
        }
    }

    private void OnVuforiaStarted()
    {
		CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
    }
}
