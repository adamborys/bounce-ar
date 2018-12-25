﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Messages;
using UnityEngine;
using UnityEngine.Networking;

public class ClientController : MonoBehaviour
{
    public static byte[] receivedBuffer;
    public NetworkController ClientNetworkController;
    private int hostId, connectionId, channelId;
    private bool isInitialized, isStarted;
    private byte sendError, receiveError, connectionError;

    void Start()
    {
        Init();
    }
    void Update()
    {
        if(isInitialized)
            UpdateServerMessage();
    }

    public void SendClientMessage(ClientMessage message)
    {
        byte[] buffer = new byte[MessageInfo.BYTE_SIZE];
        
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream(buffer);
        formatter.Serialize(stream, message);

        NetworkTransport.Send(hostId, connectionId, channelId, buffer, MessageInfo.BYTE_SIZE, out sendError);
    }
    private void UpdateServerMessage()
    {
        receivedBuffer = new byte[MessageInfo.BYTE_SIZE];
        int messageSize;
        
        NetworkEventType networkEventType = 
            NetworkTransport.Receive(out hostId, out connectionId, out channelId,
            receivedBuffer, MessageInfo.BYTE_SIZE, out messageSize, out receiveError);

        switch (networkEventType)
        {
            case NetworkEventType.Nothing:
                break;
            case NetworkEventType.ConnectEvent:
                ClientNetworkController.Log.text = "Connected to server";
                ClientNetworkController.IsConnected = true;
                break;
            case NetworkEventType.DisconnectEvent:
                ClientNetworkController.Log.text = "Disconnected from server";
                ClientNetworkController.IsConnected = false;
                break;
            case NetworkEventType.DataEvent:
                if(isStarted)
                {
                    
                }
                else
                {
                    
                }        
                break;
            case NetworkEventType.BroadcastEvent:
                ClientNetworkController.Log.text = "Unexpected broadcast";
                break;
            default:
                ClientNetworkController.Log.text = "Network Event system error";
                break;
        }
    }

    public void Init()
    {
        NetworkTransport.Init();
        ConnectionConfig config = new ConnectionConfig();
        channelId = config.AddChannel(QosType.AllCostDelivery);
        HostTopology topology = new HostTopology(config, 1);
        hostId = NetworkTransport.AddHost(topology, 0);

        NetworkTransport.Connect(hostId, ClientNetworkController.IPAddress, ClientNetworkController.Port, 0, out receiveError);
        isInitialized = true;
    }

    public void Shutdown()
    {
        NetworkTransport.Shutdown();
    }
}
