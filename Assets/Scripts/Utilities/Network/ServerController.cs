using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Messages;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public class ServerController : MonoBehaviour
{
    public static byte[] ReceivedBuffer;
    public static bool IsStarted;
    public static GameServerController GameController;
    public static int hostId, connectionId, channelId, messageSize;
    public static byte sendError, receiveError;
    public NetworkController ServerNetworkController;
    public IEnumerator ReadinessTransmitter;
    private bool isInitialized;

    void Start()
    {
        init();
        ReadinessTransmitter = TransmitServerReadyMessage();
    }
    void FixedUpdate()
    {
        if(isInitialized)
            UpdateClientMessage();
    }

    public void SendServerMessage(object message)
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
                Debug.Log(SceneManager.GetActiveScene().name);
                if(SceneManager.GetActiveScene().name == "NetworkMenu")
                {
                    ServerNetworkController.Log.text = "Client disconnected";
                    ServerNetworkController.IsConnected = false;
                }
                else
                {
                    NetworkController.Provider = null;
                    Destroy(gameObject);
                    NetworkTransport.Shutdown();
                    SceneManager.LoadScene(0);
                    NetworkController.IsServer = false;
                    ArenaController.IsReady = false;
                    ArenaController.IsOpponentReady = false;
                }
                break;
            case NetworkEventType.DataEvent:
                // Deserializing received client message
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream(ReceivedBuffer);

                if(!ArenaController.IsOpponentReady)
                {
                    if(formatter.Deserialize(stream) is ReadyMessage)
                    {
                        ArenaController.IsOpponentReady = true;
                        if(ReadinessTransmitter != null)
                            StopCoroutine(ReadinessTransmitter);
                    }
                }
                else if(ArenaController.IsReady)
                {
                    object message;
                    if((message = formatter.Deserialize(stream)) is ClientMessage)
                    {
                        GameController.RefreshArena(message as ClientMessage);
                    }
                }
                break;
            default:
                // Wróć do sceny NetworkMenu jeśli w grze
                ServerNetworkController.Log.text = "Network Event System error";
                break;
        }
    }

    public IEnumerator TransmitServerReadyMessage()
    {
        while(true)
        {
            byte[] buffer = new byte[MessageInfo.BYTE_SIZE];
            
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream(buffer);
            formatter.Serialize(stream, new ReadyMessage());

            NetworkTransport.Send(hostId, connectionId, channelId, buffer, MessageInfo.BYTE_SIZE, out sendError);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void init()
    {
        NetworkTransport.Init();
        ConnectionConfig config = new ConnectionConfig();
        channelId = config.AddChannel(QosType.AllCostDelivery);
        HostTopology topology = new HostTopology(config, 1);
        hostId = NetworkTransport.AddHost(topology, ServerNetworkController.Port, null);

        isInitialized = true;
    }
}
