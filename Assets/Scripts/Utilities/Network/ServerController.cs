using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerController : MonoBehaviour
{
    public MenuController ServerMenuController;
    private const int BYTE_SIZE = 1024;
    private byte serverChannelId;
    private int hostId;
    private bool isStarted;
    private byte networkEventError;


    void Start()
    {
        Init();
    }
    void FixedUpdate()
    {
        if(isStarted)
            UpdateClientMessage();
    }

    private void UpdateClientMessage()
    {
        int senderId, connectionId, channelId;
        byte[] buffer = new byte[BYTE_SIZE];
        int messageSize;
        
        NetworkEventType networkEventType = 
            NetworkTransport.Receive(out senderId, out connectionId, out channelId,
            buffer, BYTE_SIZE, out messageSize, out networkEventError);

        switch (networkEventType)
        {
            case NetworkEventType.Nothing:
                break;
            case NetworkEventType.ConnectEvent:
                ServerMenuController.Log.text = "Client connected";
                break;
            case NetworkEventType.DisconnectEvent:
                ServerMenuController.Log.text = "Client disconnected";
                break;
            case NetworkEventType.DataEvent:
                ServerMenuController.Log.text = "Client data received";
                break;
            case NetworkEventType.BroadcastEvent:
                ServerMenuController.Log.text = "Unexpected broadcast";
                break;
            default:
                ServerMenuController.Log.text = "Network Event system error";
                break;
        }
    }

    public void Init()
    {
        NetworkTransport.Init();
        ConnectionConfig config = new ConnectionConfig();
        serverChannelId = config.AddChannel(QosType.AllCostDelivery);
        HostTopology topology = new HostTopology(config, 1);
        hostId = NetworkTransport.AddHost(topology, ServerMenuController.Port, null);
        isStarted = true;
    }

    public void Shutdown()
    {
        NetworkTransport.Shutdown();
    }
}
