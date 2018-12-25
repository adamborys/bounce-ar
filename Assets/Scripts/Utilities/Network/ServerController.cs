using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Messages;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class ServerController : MonoBehaviour
{
    public static byte[] receivedBuffer;
    public NetworkController ServerNetworkController;
    private int hostId, serverChannelId, connectionId, channelId;
    private bool isInitialized, isStarted;
    private byte sendError, receiveError;

    void Start()
    {
        Init();
    }
    void FixedUpdate()
    {
        if(isInitialized)
            UpdateClientMessage();
    }

    public void SendServerMessage(ServerMessage message)
    {
        byte[] buffer = new byte[MessageInfo.BYTE_SIZE];
        
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream(buffer);
        formatter.Serialize(stream, message);

        NetworkTransport.Send(hostId, connectionId, channelId, buffer, MessageInfo.BYTE_SIZE, out sendError);
    }
    private void UpdateClientMessage()
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
                ServerNetworkController.Log.text = "Client connected";
                ServerNetworkController.IsConnected = true;
                break;
            case NetworkEventType.DisconnectEvent:
                ServerNetworkController.Log.text = "Client disconnected";
                ServerNetworkController.IsConnected = false;
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
                ServerNetworkController.Log.text = "Unexpected broadcast";
                break;
            default:
                ServerNetworkController.Log.text = "Network Event System error";
                break;
        }
    }

    public void Init()
    {
        NetworkTransport.Init();
        ConnectionConfig config = new ConnectionConfig();
        serverChannelId = config.AddChannel(QosType.AllCostDelivery);
        HostTopology topology = new HostTopology(config, 1);
        hostId = NetworkTransport.AddHost(topology, ServerNetworkController.Port, null);
        isInitialized = true;
    }

    public void Shutdown()
    {
        NetworkTransport.Shutdown();
    }
}
