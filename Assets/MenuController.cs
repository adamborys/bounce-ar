using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Toggle ServerToggle, ClientToggle;
    public InputField IPInput, PortInput;
    public Text Log;
    public Button PlayButton;
    // Start is called before the first frame update
    void Start()
    {
        ServerToggle.onValueChanged.AddListener(delegate {ServerSwitch();});
        ClientToggle.onValueChanged.AddListener(delegate {ClientSwitch();});
        PlayButton.onClick.AddListener(delegate {PlayClick();});
    }

    private void ClientSwitch()
    {
        if(ClientToggle.isOn)
        {
            ServerToggle.isOn = false;
            IPInput.text = "";
            IPInput.readOnly = true;
        }
        else
        {
            ServerToggle.isOn = true;
            //IPInput.text = NetworkManager.singleton.networkAddress;
            IPInput.readOnly = true;
        }
    }

    private void ServerSwitch()
    {
        if(ServerToggle.isOn)
        {
            ClientToggle.isOn = false;
            //IPInput.text = NetworkManager.singleton.networkAddress;
            IPInput.readOnly = true;
        }
        else
        {
            ClientToggle.isOn = true;
            IPInput.text = "";
            IPInput.readOnly = true;
        }
    }

    private void PlayClick()
    {
        throw new NotImplementedException();
    }
}
