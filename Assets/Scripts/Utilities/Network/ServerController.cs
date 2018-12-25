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
    public static byte[] ReceivedBuffer;
    public static bool IsStarted;
    public static GameServerController GameController;
    public static int hostId, connectionId, channelId, messageSize;
    public static byte sendError, receiveError;
    public NetworkController ServerNetworkController;
    private bool isInitialized;

    void Start()
    {
        Init();
    }
    void FixedUpdate()
    {
        if(isInitialized)
            UpdateClientMessage();
    }

    public static void ReceiveReadyMessage()
    {
        NetworkEventType networkEventType = 
            NetworkTransport.Receive(out hostId, out connectionId, out channelId,
            ReceivedBuffer, MessageInfo.INITIAL_BYTE_SIZE, out messageSize, out receiveError);
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
        ReceivedBuffer = new byte[MessageInfo.BYTE_SIZE];
        
        NetworkEventType networkEventType = 
            NetworkTransport.Receive(out hostId, out connectionId, out channelId,
            ReceivedBuffer, MessageInfo.BYTE_SIZE, out messageSize, out receiveError);

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
                if(IsStarted)
                {
                    GameController.RefreshArena();
                }
                else
                {
                    ArenaController.IsOpponentReady = true;
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
        channelId = config.AddChannel(QosType.AllCostDelivery);
        HostTopology topology = new HostTopology(config, 1);
        hostId = NetworkTransport.AddHost(topology, ServerNetworkController.Port, null);
        isInitialized = true;
    }

    public void Shutdown()
    {
        NetworkTransport.Shutdown();
    }
}
