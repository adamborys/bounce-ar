using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Messages;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.EventSystems;

public class GameClientController : MonoBehaviour
{
    private Transform serverBallTransform, clientBallTransform;
    private GameObject serverBarrier, clientBarrier;
    private Vector3 firstPosition, secondPosition, lastFirstPosition, lastSecondPosition;
    private bool isNewBarrier = true;
    
    void Start()
    {
        serverBallTransform = transform.GetChild(0);
        clientBallTransform = transform.GetChild(1);
    }
    void Update()
    {
        /*
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
        */
    }
    void FixedUpdate()
    {
        if(ArenaController.IsReady && ArenaController.IsOpponentReady)
        {
            // Sending client user input info
            ClientMessage clientMessage = 
                new ClientMessage(lastFirstPosition, lastSecondPosition);
            NetworkController.Provider.GetComponent<ClientController>().SendClientMessage(clientMessage);
        }
    }

    public void RefreshArena(ServerMessage serverMessage)
    {
        Debug.Log("RefreshArena");
        // Manipulating ball transforms according to server message
        serverBallTransform.localPosition = new Vector3(serverMessage.ServerBallX, 0.004f, serverMessage.ServerBallY);
        clientBallTransform.localPosition = new Vector3(serverMessage.ClientBallX, 0.004f, serverMessage.ClientBallY);
        /*
        // Building barriers according to server message
        Destroy(serverBarrier);
        Destroy(clientBarrier);
        serverBarrier = BuildBarrier(serverMessage.ServerFirst, serverMessage.ServerFirst, "ServerBarrier");
        clientBarrier = BuildBarrier(serverMessage.ClientFirst, serverMessage.ClientFirst, "ClientBarrier");
        */
    }

    private IEnumerator DelayedBarrierRefresh()
    {
        yield return new WaitForSeconds(1);
        isNewBarrier = true;
    }
    private GameObject BuildBarrier(Vector3 firstPosition, Vector3 secondPosition, string name)
    {
        firstPosition.y = secondPosition.y = 0.0035f;
        Vector3 midpoint = (secondPosition - firstPosition) / 2 + firstPosition;

        GameObject barrier = GameObject.CreatePrimitive(PrimitiveType.Cube);
        barrier.transform.SetParent(transform, false);
        barrier.name = name;
        barrier.transform.localPosition = midpoint;
        float barrierLength = Vector3.Distance(firstPosition, secondPosition);
        barrier.transform.localScale = 
            new Vector3(0.0002f, 0.005f, Mathf.Clamp(barrierLength, 0.005f, 0.03f));
        barrier.transform.LookAt(transform.TransformPoint(secondPosition), transform.up);
        return barrier;
    }
}
