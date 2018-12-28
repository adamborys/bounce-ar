using UnityEngine;

namespace Messages
{
    public class MessageInfo
    {
        public const int BYTE_SIZE = 1024;
    }
    [System.Serializable]
    public class ReadyMessage
    {
        public string Message { get => message; }
        private string message;
        public ReadyMessage()
        {
            message = "ready";
        }
    }
    [System.Serializable]
    public class ClientMessage
    {
        private float firstX, firstY, secondX, secondY;

        public ClientMessage(Vector3 first, Vector3 second)
        {
            firstX = first.x;
            firstY = first.z;
            secondX = second.x;
            secondY = second.z;
        }

        public float FirstX { get => firstX; }
        public float FirstY { get => firstY; }
        public float SecondX { get => secondX; }
        public float SecondY { get => secondY; }
    }
    [System.Serializable]
    public class ServerMessage
    {
        private float serverFirstX, serverFirstY, serverSecondX, serverSecondY,
        clientFirstX, clientFirstY, clientSecondX, clientSecondY, 
        serverBallX, serverBallY, clientBallX, clientBallY;

        public ServerMessage(Vector3 serverFirst, Vector3 serverSecond,
                            Vector3 clientFirst, Vector3 clientSecond,
                            Vector3 serverBall, Vector3 clientBall)
        {
            serverFirstX = serverFirst.x;
            serverFirstY = serverFirst.z;
            serverSecondX = serverSecond.x;
            serverSecondY = serverSecond.z;
            clientFirstX = clientFirst.x;
            clientFirstY = clientFirst.z;
            clientSecondX = clientSecond.x;
            clientSecondY = clientSecond.z;
            serverBallX = serverBall.x;
            serverBallY = serverBall.z;
            clientBallX = clientBall.x;
            clientBallY = clientBall.z;    
        }

        public float ServerFirstX { get => serverFirstX; }
        public float ServerFirstY { get => serverFirstY; }
        public float ServerSecondX { get => serverSecondX; }
        public float ServerSecondY { get => serverSecondY; }
        public float ClientFirstX { get => clientFirstX; }
        public float ClientFirstY { get => clientFirstY; }
        public float ClientSecondX { get => clientSecondX; }
        public float ClientSecondY { get => clientSecondY; }
        public float ServerBallX { get => serverBallX; }
        public float ServerBallY { get => serverBallY; }
        public float ClientBallX { get => clientBallX; }
        public float ClientBallY { get => clientBallY; }
    }
}