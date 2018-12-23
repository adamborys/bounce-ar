using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ClientController : MonoBehaviour
{
    public MenuController ClientMenuController;
    private const int BYTE_SIZE = 1024;
    private byte channelId;
    private int hostId;
    private byte error;
    private bool isStarted;
    private byte networkEventError;

    void Start()
    {
        Init();
    }
    void Update()
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
                ClientMenuController.Log.text = "Connected to server";
                ClientMenuController.IsConnected = true;
                break;
            case NetworkEventType.DisconnectEvent:
                ClientMenuController.Log.text = "Disconnected from server";
                ClientMenuController.IsConnected = false;
                break;
            case NetworkEventType.DataEvent:
                ClientMenuController.Log.text = "Server data received";
                break;
            case NetworkEventType.BroadcastEvent:
                ClientMenuController.Log.text = "Unexpected broadcast";
                break;
            default:
                ClientMenuController.Log.text = "Network Event system error";
                break;
        }
    }

    public NetworkError Init()
    {
        NetworkTransport.Init();
        ConnectionConfig config = new ConnectionConfig();
        channelId = config.AddChannel(QosType.AllCostDelivery);
        HostTopology topology = new HostTopology(config, 1);
        hostId = NetworkTransport.AddHost(topology, 0);

        NetworkTransport.Connect(hostId, ClientMenuController.IPAddress, ClientMenuController.Port, 0, out error);
        isStarted = true;
        return (NetworkError)error;
    }

    public void Shutdown()
    {
        NetworkTransport.Shutdown();
    }
}
