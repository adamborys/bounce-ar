using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MenuController : MonoBehaviour
{
    public bool IsConnected
    {
        get
        {
            return isConnected;
        }
        set
        {

            if (!value)
                if (ServerToggle.isOn)
                    submitButtonLabel.text = "Start";
                else
                    submitButtonLabel.text = "Connect";
            else
                submitButtonLabel.text = "Play";

            isConnected = value;
        }
    }
    private bool isConnected;
    public string IPAddress;
    public int Port;
    public Toggle ServerToggle, ClientToggle;
    public InputField IPInput, PortInput;
    public Text Log;
    public Button SubmitButton;
    private Text submitButtonLabel;
    private GameObject networkProvider;
    private string ipPattern =
        @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";

    // Start is called before the first frame update
    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        ServerToggle.onValueChanged.AddListener(delegate { ServerSwitch(); });
        ClientToggle.onValueChanged.AddListener(delegate { ClientSwitch(); });
        SubmitButton.onClick.AddListener(delegate { PlayClick(); });
        submitButtonLabel = SubmitButton.GetComponentInChildren<Text>();
    }

    #region UI listeners  
    private void ClientSwitch()
    {
        ResetConnection();
        if (ClientToggle.isOn)
        {
            ServerToggle.isOn = false;
            IPInput.text = "";
            IPInput.readOnly = false;
            submitButtonLabel.text = "Connect";
        }
        else
        {
            ServerToggle.isOn = true;
            IPInput.text = GetLocalIPAddress();
            IPInput.readOnly = true;
            submitButtonLabel.text = "Start";
        }
    }

    private void ServerSwitch()
    {
        ResetConnection();
        if (ServerToggle.isOn)
        {
            ClientToggle.isOn = false;
            IPInput.text = GetLocalIPAddress();
            IPInput.readOnly = true;
            submitButtonLabel.text = "Start";
        }
        else
        {
            ClientToggle.isOn = true;
            IPInput.text = "";
            IPInput.readOnly = false;
            submitButtonLabel.text = "Connect";
        }
    }

    private void PlayClick()
    {
        if (!isConnected)
        {
            if (int.TryParse(PortInput.text, out Port) && Port < 65536)
            {
                if (ServerToggle.isOn)
                {
                    ServerInit();
                    Log.text = "Awaiting client connection";
                }
                else
                {
                    Regex regex = new Regex(ipPattern);
                    if (regex.IsMatch(IPInput.text))
                    {
                        IPAddress = IPInput.text;
                        ClientInit();
                        Log.text = "Trying to connect with server";
                    }
                    else
                    {
                        Log.text = "Invalid IP address!";
                    }
                }
            }
            else
            {
                Log.text = "Invalid port number!";
            }
        }
        else
        {
            //DontDestroy
            // Go to TargetBuilder
        }
    }

    #endregion

    private string GetLocalIPAddress()
    {
        IPHostEntry host;
        string localIP = "?";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
            }
        }
        return localIP;
    }

    public void ServerInit()
    {
        networkProvider = new GameObject("Server");
        ServerController serverController = networkProvider.AddComponent<ServerController>();
        serverController.ServerMenuController = this;
    }

    public void ClientInit()
    {
        networkProvider = new GameObject("Client");
        ClientController clientController = networkProvider.AddComponent<ClientController>();
        clientController.ClientMenuController = this;
    }

    public void ResetConnection()
    {
        if (networkProvider != null)
        {
            if (ServerToggle.isOn)
            {
                networkProvider.AddComponent<ServerController>().Shutdown();
            }
            else
            {
                networkProvider.AddComponent<ClientController>().Shutdown();
            }
            Destroy(networkProvider);
        }
        Log.text = "Enter data and click to establish connection";
    }
}
