using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerController : MonoBehaviour
{
    private byte channelId;
    private int hostId;
    public void Init(int port)
    {
        NetworkTransport.Init();
        ConnectionConfig config = new ConnectionConfig();
        channelId = config.AddChannel(QosType.AllCostDelivery);
        HostTopology topology = new HostTopology(config, 1);
        hostId = NetworkTransport.AddHost(topology, port, null);
    }

    public void Shutdown()
    {
        NetworkTransport.Shutdown();
    }
}
