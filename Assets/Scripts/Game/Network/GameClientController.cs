﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Messages;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameClientController : MonoBehaviour
{
    public static Text ServerScoreText, ClientScoreText;
    private Transform arenaTransform;
    private Transform arenaProxyTransform;
    private Transform serverBallTransform, clientBallTransform;
    private GameObject serverBarrier, clientBarrier;
    private Vector3 firstPosition, secondPosition, 
    lastFirstPosition = new Vector3(-0.01f,0,0.06f), lastSecondPosition = new Vector3(-0.04f,0,0.06f);
    private bool isNewBarrier = true;

    // Iterator for reducing network overload
    private byte networkIterator = 0;
    
    void Start()
    {
        serverBallTransform = transform.GetChild(0);
        clientBallTransform = transform.GetChild(1);

        arenaTransform = GameObject.Find("Arena").transform;
        Transform canvasTransform = GameObject.Find("GameCanvas").transform;
        Text[] textComponents = canvasTransform.GetComponentsInChildren<Text>();
        ServerScoreText = textComponents[0];
        ClientScoreText = textComponents[1];
        
        arenaProxyTransform = GameObject.Find("ArenaProxy").transform;
    }
    // Maintaining game imput
    void Update()
    {
        if(ArenaController.IsReady && ArenaController.IsOpponentReady)
        {
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
                LayerMask mask = LayerMask.GetMask("Floor");
                // Ignoring other layers than "Floor"
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
                {
                    if (isNewBarrier)
                    {
                        firstPosition = arenaTransform.InverseTransformPoint(hit.point);
                        StartCoroutine(DelayedBarrierRefresh());
                    }
                    else
                    {
                        StopCoroutine(DelayedBarrierRefresh());
                        secondPosition = arenaTransform.InverseTransformPoint(hit.point);

                        lastFirstPosition = firstPosition;
                        lastSecondPosition = secondPosition;
                    }
                    isNewBarrier = !isNewBarrier;
                }
            }
        }

        CopyTransformData(arenaProxyTransform.GetChild(0), arenaTransform.GetChild(0));
        CopyTransformData(arenaProxyTransform.GetChild(1), arenaTransform.GetChild(1));
        CopyTransformData(arenaProxyTransform.GetChild(2), arenaTransform.GetChild(2));
        CopyTransformData(arenaProxyTransform.GetChild(3), arenaTransform.GetChild(3));
    }
    // Used for network communication
    void FixedUpdate()
    {
        if(ArenaController.IsReady && ArenaController.IsOpponentReady)
        {
            // Sending client user input info
            if(networkIterator == 0)
            {
                // Sending client user input info
                ClientMessage clientMessage = 
                    new ClientMessage(lastFirstPosition, lastSecondPosition);
                NetworkController.Provider.GetComponent<ClientController>().SendClientMessage(clientMessage);
            }

            networkIterator++;
            networkIterator %= 3;
        }
    }

    public void RefreshArena(ServerMessage serverMessage)
    {
        /* 
        // Manipulating ball transforms according to server message
        serverBallTransform.localPosition = new Vector3(serverMessage.ServerBallX, 0.004f, serverMessage.ServerBallY);
        clientBallTransform.localPosition = new Vector3(serverMessage.ClientBallX, 0.004f, serverMessage.ClientBallY);

        // Building barriers according to server message
        Destroy(serverBarrier);
        Destroy(clientBarrier);
        serverBarrier = BuildBarrier(new Vector2(serverMessage.ServerFirstX, serverMessage.ServerFirstY), 
                                    new Vector2(serverMessage.ServerSecondX, serverMessage.ServerSecondY), 
                                    "ServerBarrier");
        clientBarrier = BuildBarrier(new Vector2(serverMessage.ClientFirstX, serverMessage.ClientFirstY), 
                                    new Vector2(serverMessage.ClientSecondX, serverMessage.ClientSecondY), 
                                    "ClientBarrier");
        */
    }

    private void CopyTransformData(Transform from, Transform to)
    {
        to.position = new Vector3(from.position.x, 0.5f, from.position.y);
    }
   
    // Neutralising random clicks
    private IEnumerator DelayedBarrierRefresh()
    {
        yield return new WaitForSeconds(1);
        isNewBarrier = true;
    }

    // Moving barrier according to two touch/mouse coordinates
    private void MoveBarrier(Vector2 firstPosition, Vector2 secondPosition, string name)
    {
        /*
        Vector3 firstPosition3D = new Vector3(firstPosition2D.x, 0.0035f, firstPosition2D.y);
        Vector3 secondPosition3D = new Vector3(secondPosition2D.x, 0.0035f, secondPosition2D.y);
        
        Vector3 midpoint = (secondPosition3D - firstPosition3D) / 2 + firstPosition3D;

        GameObject barrier = GameObject.CreatePrimitive(PrimitiveType.Cube);
        barrier.transform.SetParent(transform, false);
        barrier.name = name;
        barrier.transform.localPosition = midpoint;
        float barrierLength = Vector3.Distance(firstPosition3D, secondPosition3D);
        barrier.transform.localScale = 
            new Vector3(0.0002f, 0.005f, Mathf.Clamp(barrierLength, 0.005f, 0.03f));
        barrier.transform.LookAt(transform.TransformPoint(secondPosition3D), transform.up);
        return barrier;
        */
    }
}