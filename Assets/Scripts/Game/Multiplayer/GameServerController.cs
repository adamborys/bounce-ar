using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Messages;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameServerController : MonoBehaviour
{
    public static Vector3 lastClientFirstPosition, lastClientSecondPosition;
    Transform serverBallTransform, clientBallTransform;
    GameObject serverBarrier, clientBarrier;
    Vector3 firstPosition, secondPosition, lastFirstPosition, lastSecondPosition;
    bool isNewBarrier = true;
    
    void Start()
    {
        serverBallTransform = transform.GetChild(0);
        clientBallTransform = transform.GetChild(1);
        StartCoroutine(DelayedBallsLaunch());
    }
    void Update()
    {
        /* 
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
        */
    }
    void FixedUpdate()
    {
        if(ArenaController.IsReady && ArenaController.IsOpponentReady)
        {
            // Collision info processing (game logic)
            

            // Sending client user input info
            ServerMessage serverMessage = 
                new ServerMessage(lastFirstPosition, lastSecondPosition, 
                                lastClientFirstPosition, lastClientSecondPosition,
                                serverBallTransform.localPosition, clientBallTransform.localPosition);
            NetworkController.Provider.GetComponent<ServerController>().SendServerMessage(serverMessage);
        }
    }

    public void RefreshArena(ClientMessage clientMessage)
    {
        Debug.Log("RefreshArena");
        /* 
        lastClientFirstPosition = clientMessage.First;
        lastClientSecondPosition = clientMessage.Second;
        // Building barriers according to server message
        Destroy(clientBarrier);
        clientBarrier = BuildBarrier(clientMessage.First, clientMessage.Second, "ClientBarrier");
        */
    }

    private IEnumerator DelayedBarrierRefresh()
    {
        yield return new WaitForSeconds(1);
        isNewBarrier = true;
    }
    private IEnumerator DelayedBallsLaunch()
    {
        while(!(ArenaController.IsReady && ArenaController.IsOpponentReady))
        {
            yield return null;
        }
        serverBallTransform.gameObject.GetComponent<BallController>().LaunchBall();
        clientBallTransform.gameObject.GetComponent<BallController>().LaunchBall();
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
