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
    public Toggle ServerToggle, ClientToggle;
    public InputField IPInput, PortInput;
    public Text Log;
    public Button PlayButton;
    private GameObject networkProvider;
    private string ipPattern = 
        @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
    // Start is called before the first frame update
    void Start()
    {
        ServerToggle.onValueChanged.AddListener(delegate {ServerSwitch();});
        ClientToggle.onValueChanged.AddListener(delegate {ClientSwitch();});
        PlayButton.onClick.AddListener(delegate {PlayClick();});
    }

    #region UI listeners  
    private void ClientSwitch()
    {
        if(ClientToggle.isOn)
        {
            ServerToggle.isOn = false;
            IPInput.text = "";
            IPInput.readOnly = false;
        }
        else
        {
            ServerToggle.isOn = true;
            IPInput.text = GetLocalIPAddress();
            IPInput.readOnly = true;
        }
    }

    private void ServerSwitch()
    {
        if(ServerToggle.isOn)
        {
            ClientToggle.isOn = false;
            IPInput.text = GetLocalIPAddress();
            IPInput.readOnly = true;
        }
        else
        {
            ClientToggle.isOn = true;
            IPInput.text = "";
            IPInput.readOnly = false;
        }
    }

    private void PlayClick()
    {
        int port;
        if(int.TryParse(PortInput.text, out port) && port < 65536)
        {
            if(ServerToggle.isOn)
            {
                ServerInit(port);
            }
            else
            {
                Regex regex = new Regex(ipPattern);
                if(regex.IsMatch(IPInput.text))
                {
                    Log.text = ClientInit(IPInput.text, port).ToString();
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
        //DontDestroy
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

    public void ServerInit(int port)
    {
        networkProvider = new GameObject("Server");
        ServerController serverController = networkProvider.AddComponent<ServerController>();

        serverController.Init(port);
    }

    public NetworkError ClientInit(string ip, int port)
    {
        networkProvider = new GameObject("Client");
        ClientController clientController = networkProvider.AddComponent<ClientController>();

        return clientController.Init(ip, port);
    }
}
