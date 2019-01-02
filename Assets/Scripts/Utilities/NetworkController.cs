using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NetworkController : MonoBehaviour
{
    public static bool IsServer;
    public static GameObject Provider;
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

        // Defaults for convenience
        IPInput.text = "192.168.43.1";
        PortInput.text = "11113";
    }

    #region UI listeners  

    // Changing UI functionality and resetting connection due to modified server-client choice
    private void ClientSwitch()
    {
        ResetConnection();
        if (ClientToggle.isOn)
        {
            ServerToggle.isOn = false;
            IsServer = false;
            IPInput.readOnly = false;
            IPInput.text = "192.168.43.1";
            submitButtonLabel.text = "Connect";
        }
        else
        {
            ServerToggle.isOn = true;
            IsServer = true;
            IPInput.readOnly = true;
            IPInput.text = GetLocalIPAddress();
            submitButtonLabel.text = "Start";
        }
        Log.text = "Enter data and click to establish connection";
    }

    private void ServerSwitch()
    {
        ResetConnection();
        if (ServerToggle.isOn)
        {
            ClientToggle.isOn = false;
            IPInput.readOnly = true;
            IsServer = true;
            IPInput.text = GetLocalIPAddress();
            submitButtonLabel.text = "Start";
        }
        else
        {
            ClientToggle.isOn = true;
            IPInput.readOnly = false;
            IsServer = false;
            IPInput.text = "192.168.43.1";
            submitButtonLabel.text = "Connect";
        }
        Log.text = "Enter data and click to establish connection";
    }

    // 1. Parsing text input and if correct - CONNECTING
    // 2. IF CONNECTED - Parsing text input and if correct - LOADING TargetBuilder SCENE
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
            DontDestroyOnLoad(Provider);
            SceneManager.LoadScene("TargetBuilder", LoadSceneMode.Single);
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
        ResetConnection();
        Provider = new GameObject("Server");
        ServerController serverController = Provider.AddComponent<ServerController>();
        serverController.ServerNetworkController = this;
    }

    public void ClientInit()
    {
        ResetConnection();
        Provider = new GameObject("Client");
        ClientController clientController = Provider.AddComponent<ClientController>();
        clientController.ClientNetworkController = this;
    }

    public void ResetConnection()
    {
        if (Provider != null)
        {
            NetworkTransport.Shutdown();
            Destroy(Provider);
        }
    }
}
