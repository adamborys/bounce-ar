using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Messages;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ClientController : MonoBehaviour
{
    public static byte[] ReceivedBuffer;
    public static bool IsStarted;
    public static GameClientController GameController;
    public static int hostId, connectionId, channelId, messageSize;
    public static byte sendError, receiveError;
    public NetworkController ClientNetworkController;
    public IEnumerator ReadinessTransmitter;
    private bool isInitialized;

    void Start()
    {
        init();
        ReadinessTransmitter = TransmitClientReadyMessage();
    }
    void FixedUpdate()
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
        ReceivedBuffer = new byte[MessageInfo.BYTE_SIZE];
        int messageSize;
        
        NetworkEventType networkEventType = 
                NetworkTransport.Receive(out hostId, out connectionId, out channelId,
                ReceivedBuffer, MessageInfo.BYTE_SIZE, out messageSize, out receiveError);

        switch (networkEventType)
        {
            case NetworkEventType.Nothing:
                break;
            case NetworkEventType.ConnectEvent:
                ClientNetworkController.Log.text = "Connected to server";
                ClientNetworkController.IsConnected = true;
                break;
            case NetworkEventType.DisconnectEvent:
                NetworkController.Provider = null;
                Destroy(gameObject);
                NetworkTransport.Shutdown();
                if(SceneManager.GetActiveScene().name == "NetworkMenu")
                {
                    ClientNetworkController.Log.text = "Server disconnected";
                    ClientNetworkController.IsConnected = false;
                }
                else
                {
                    SceneManager.LoadScene(0);
                    ArenaController.IsReady = false;
                    ArenaController.IsOpponentReady = false;
                }
                break;
            case NetworkEventType.DataEvent:
                // Deserializing received server game info
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
                    object message = formatter.Deserialize(stream);
                    if(message is ServerMessage)
                    {
                        GameController.RefreshArena(message as ServerMessage);
                    }
                    else if(message is ScoreMessage)
                    {
                        ScoreMessage scoreMessage = (message as ScoreMessage);
                        GameClientController.ServerScoreText.text = scoreMessage.ServerScore.ToString();
                        GameClientController.ClientScoreText.text = scoreMessage.ClientScore.ToString();
                        if(scoreMessage.IsCountdown)
                        {
                            
                        }
                        else //InGame score message
                        {

                        }
                    }
                }
                break;
            default:
                // Wróć do sceny NetworkMenu jeśli w grze
                ClientNetworkController.Log.text = "Network Event system error";
                break;
        }
    }

    public IEnumerator TransmitClientReadyMessage()
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
        hostId = NetworkTransport.AddHost(topology, 0);

        NetworkTransport.Connect(hostId, ClientNetworkController.IPAddress, ClientNetworkController.Port, 0, out receiveError);
        isInitialized = true;
    }
}
