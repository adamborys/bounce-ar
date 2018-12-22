using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ClientController : MonoBehaviour
{
    private byte channelId;
    private int hostId;
    private byte error;
    public NetworkError Init(string ip, int port)
    {
        NetworkTransport.Init();
        ConnectionConfig config = new ConnectionConfig();
        channelId = config.AddChannel(QosType.AllCostDelivery);
        HostTopology topology = new HostTopology(config, 1);
        hostId = NetworkTransport.AddHost(topology, 0);

        NetworkTransport.Connect(hostId, ip, port, 0, out error);
        return (NetworkError)error;
    }

    public void Shutdown()
    {
        NetworkTransport.Shutdown();
    }
}
