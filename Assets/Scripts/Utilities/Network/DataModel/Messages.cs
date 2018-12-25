using UnityEngine;

namespace Messages
{
    public class MessageInfo
    {
        public const int BYTE_SIZE = 1024;
        public const int INITIAL_BYTE_SIZE = 512;
    }
    [System.Serializable]
    public class ClientMessage
    {
        public Vector3 first;
        public Vector3 second;

        public ClientMessage(Vector3 first, Vector3 second)
        {
            this.first = first;
            this.second = second;
        }
    }
    [System.Serializable]
    public class ServerMessage
    {
        public Vector3 serverFirst;
        public Vector3 serverSecond;
        public Vector3 clientFirst;
        public Vector3 clientSecond;
        public Vector3 serverBallPosition;
        public Vector3 clientBallPosition;

        public ServerMessage(Vector3 serverFirst, Vector3 serverSecond,
                            Vector3 clientFirst, Vector3 clientSecond,
                            Vector3 serverBallPosition, Vector3 clientBallPosition)
        {
            this.serverFirst = serverFirst;
            this.serverSecond = serverSecond;
            this.clientFirst = clientFirst;
            this.clientSecond = clientSecond;
            this.serverBallPosition = serverBallPosition;
            this.clientBallPosition = clientBallPosition;
        }
    }
}