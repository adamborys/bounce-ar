﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Messages;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameServerController : MonoBehaviour
{
    public Text ServerScoreText, ClientScoreText;
    public int ServerScore
    {
        get 
        { 
            return serverScore; 
        }
        set
        {
            ServerScoreText.text = (serverScore = value).ToString();
            ScoreMessage scoreMessage = new ScoreMessage(serverScore, clientScore, false);
            NetworkController.Provider.GetComponent<ServerController>().SendServerMessage(scoreMessage);
        }
    }
    public int ClientScore
    {
        get 
        { 
            return clientScore; 
        }
        set
        {
            ClientScoreText.text = (clientScore = value).ToString();
            ScoreMessage scoreMessage = new ScoreMessage(serverScore, clientScore, false);
            NetworkController.Provider.GetComponent<ServerController>().SendServerMessage(scoreMessage);
        }
    }
    private int serverScore, clientScore;
    private Transform serverBallTransform, clientBallTransform;
    private GameObject serverBarrier, clientBarrier;
    private BallController serverBallController, clientBallController;
    private Vector3 firstPosition, secondPosition, 
                    lastFirstPosition = new Vector3(0.01f,0,-0.06f), 
                    lastSecondPosition = new Vector3(0.04f,0,-0.06f),
                    lastClientFirstPosition = new Vector3(-0.01f,0,0.06f), 
                    lastClientSecondPosition = new Vector3(-0.04f,0,0.06f),

                    initialServerBallPosition = new Vector3(-0.022f, 0.004f, -0.07f),
                    initialClientBallPosition = new Vector3(-0.022f, 0.004f, 0.07f);

    private bool isNewBarrier = true;
    private bool isScoreSent = true;

    // Iterator for reducing network overload
    private byte networkIterator = 0;

    void Start()
    {
        serverBallTransform = transform.GetChild(0);
        clientBallTransform = transform.GetChild(1);
        serverBallController = serverBallTransform.gameObject.AddComponent<BallController>();
        clientBallController = clientBallTransform.gameObject.AddComponent<BallController>();
        serverBallController.Direction = Vector3.forward;
        clientBallController.Direction = -Vector3.forward;
        serverBallController.Speed = clientBallController.Speed = 0.01f;
        StartCoroutine(DelayedBallsLaunch());

        Transform canvasTransform = transform.GetChild(6);
        Text[] textComponents = canvasTransform.GetComponentsInChildren<Text>();
        ServerScoreText = textComponents[0];
        ClientScoreText = textComponents[1];
    }
    // Maintaining game imput
    void Update()
    {
        if(ArenaController.IsReady && ArenaController.IsOpponentReady)
        {
            // IF READY SIGNAL SENT AND SERVER READY
            int pointerID;
            if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                pointerID = Input.GetTouch(0).fingerId;
            else if(Application.platform == RuntimePlatform.WindowsEditor)
                pointerID = -1;
            else
                return;

            if (Input.GetMouseButtonDown(0) && // Ignoring UI touch
            !EventSystem.current.IsPointerOverGameObject(pointerID))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                LayerMask mask = LayerMask.GetMask("Default");
                // Ignoring other layers than "Default"
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
                {
                    if (isNewBarrier)
                    {
                        firstPosition = transform.InverseTransformPoint(hit.point);
                        StartCoroutine(DelayedBarrierRefresh());
                    }
                    else
                    {
                        StopCoroutine(DelayedBarrierRefresh());
                        secondPosition = transform.InverseTransformPoint(hit.point);

                        lastFirstPosition = firstPosition;
                        lastSecondPosition = secondPosition;
                    }
                    isNewBarrier = !isNewBarrier;
                }
            }
        }
    }
    // Used for network communication
    void FixedUpdate()
    {
        if(ArenaController.IsReady && ArenaController.IsOpponentReady)
        {
            if(!isScoreSent)
            {

                isScoreSent = true;
            }

            // Sending server user input info
            if(networkIterator == 0)
            {
                ServerMessage serverMessage = 
                    new ServerMessage(lastFirstPosition, lastSecondPosition, 
                                    lastClientFirstPosition, lastClientSecondPosition,
                                    serverBallTransform.localPosition, clientBallTransform.localPosition);
                NetworkController.Provider.GetComponent<ServerController>().SendServerMessage(serverMessage);
            }

            networkIterator++;
            networkIterator %= 3;
        }
    }

    public void RefreshArena(ClientMessage clientMessage)
    {
        Debug.Log("RefreshArena");
        
        lastClientFirstPosition = new Vector3(clientMessage.FirstX, 0, clientMessage.FirstY);
        lastClientSecondPosition = new Vector3(clientMessage.SecondX, 0, clientMessage.SecondY);

        Destroy(serverBarrier);
        Destroy(clientBarrier);
        clientBarrier = BuildBarrier(new Vector2(lastClientFirstPosition.x, lastClientFirstPosition.z), 
                                    new Vector2(lastClientSecondPosition.x, lastClientSecondPosition.z), 
                                    "ClientBarrier");
        serverBarrier = BuildBarrier(new Vector2(lastFirstPosition.x, lastFirstPosition.z), 
                                    new Vector2(lastSecondPosition.x, lastSecondPosition.z), 
                                    "ClientBarrier");
    }
    public void OnBallCollision(Transform ballTransform, string collidingObjectName)
    {
        if(ballTransform == serverBallTransform && collidingObjectName == "ClientWall")
        {
            ServerScore++;
        }
        if(ballTransform == clientBallTransform && collidingObjectName == "ServerWall")
        {
            ClientScore++;
        }
    }
    public void OnBarrierCollision(GameObject barrierObject, GameObject collidingObject)
    {
        
    }

    private IEnumerator DelayedBallsLaunch()
    {
        while(!(ArenaController.IsReady && ArenaController.IsOpponentReady))
        {
            yield return null;
        }
        for(int i = 3; i >= 0; i--)
        {
            ServerScoreText.text = i.ToString();
            ClientScoreText.text = i.ToString();
            ScoreMessage countdownMessage = new ScoreMessage(i, i, true);
            NetworkController.Provider.GetComponent<ServerController>().SendServerMessage(countdownMessage);
            yield return new WaitForSeconds(1);
        }
        serverBallTransform.gameObject.GetComponent<BallController>().LaunchBall();
        clientBallTransform.gameObject.GetComponent<BallController>().LaunchBall();
    }
    private void BallsRelaunch()
    {
        serverBallTransform.localPosition = initialServerBallPosition;
        clientBallTransform.localPosition = initialClientBallPosition;
        serverBallController.Direction = Vector3.forward;
        clientBallController.Direction = -Vector3.forward;
        serverBallController.Speed = 0.01f;
        clientBallController.Speed = 0.01f;

        StartCoroutine(DelayedBallsLaunch());
    }
    // Neutralising random clicks
    private IEnumerator DelayedBarrierRefresh()
    {
        yield return new WaitForSeconds(1);
        isNewBarrier = true;
    }
    // Building barrier from two touch/mouse coordinates
    private GameObject BuildBarrier(Vector2 firstPosition2D, Vector2 secondPosition2D, string name)
    {
        Vector3 firstPosition3D = new Vector3(firstPosition2D.x, 0.0035f, firstPosition2D.y);
        Vector3 secondPosition3D = new Vector3(secondPosition2D.x, 0.0035f, secondPosition2D.y);
        
        Vector3 midpoint = (secondPosition3D - firstPosition3D) / 2 + firstPosition3D;

        GameObject barrier = GameObject.CreatePrimitive(PrimitiveType.Cube);
        barrier.AddComponent<BarrierController>();
        barrier.transform.SetParent(transform, false);
        barrier.name = name;
        barrier.transform.localPosition = midpoint;
        float barrierLength = Vector3.Distance(firstPosition3D, secondPosition3D);
        barrier.transform.localScale = 
            new Vector3(0.001f, 0.005f, Mathf.Clamp(barrierLength, 0.005f, 0.03f));
        barrier.transform.LookAt(transform.TransformPoint(secondPosition3D), transform.up);
        return barrier;
    }
}
