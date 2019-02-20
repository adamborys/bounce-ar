using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Messages;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
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

            if(serverScore > 2)
            {
                serverBallController.FreezeBall();
                clientBallController.FreezeBall();
                ServerScoreText.text = "Win";
                ClientScoreText.text = "Loss";
                StartCoroutine(RestartGame());
            }
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
            
            if(clientScore > 2)
            {
                //serverBallController.FreezeBall();
                //clientBallController.FreezeBall();
                ClientScoreText.text = "Win";
                ServerScoreText.text = "Loss";
                StartCoroutine(RestartGame());
            }
        }
    }
    private int serverScore, clientScore;
    private Transform arenaTransform;
    private Transform arenaProxyTransform;
    private Transform serverBallTransform, clientBallTransform,
                      serverBarrierTransform, clientBarrierTransform;
    private BallController serverBallController, clientBallController;
    private Vector3 firstPosition, secondPosition;

    private Coroutine latestTransition;
    private bool isNewBarrier = true;
    private ContactPoint2D[] barrierContacts = new ContactPoint2D[5];

    void Start()
    {
        serverBallTransform = transform.GetChild(0);
        clientBallTransform = transform.GetChild(1);
        serverBarrierTransform = transform.GetChild(2);
        clientBarrierTransform = transform.GetChild(3);

        arenaTransform = GameObject.Find("Arena").transform;

        Transform canvasTransform = GameObject.Find("GameCanvas").transform;
        Text[] textComponents = canvasTransform.GetComponentsInChildren<Text>();
        ServerScoreText = textComponents[0];
        ClientScoreText = textComponents[1];
        
        arenaProxyTransform = GameObject.Find("ArenaProxy").transform;
        
        serverBallController = serverBallTransform.gameObject.GetComponent<BallController>();
        clientBallController = clientBallTransform.gameObject.GetComponent<BallController>();
        serverBallController.ballBody.velocity = BallController.ServerStartVelocity;
        clientBallController.ballBody.velocity = BallController.ClientStartVelocity;

        //StartCoroutine(DelayedBallsLaunch());
    }

    // Maintaining game imput
    void Update()
    {
        //if(ArenaController.IsReady && ArenaController.IsOpponentReady)
        {
            if (Input.GetMouseButtonDown(0))
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
                        Vector3 midpoint = (secondPosition - firstPosition) / 2 + firstPosition;

                        if(firstPosition.x > secondPosition.x)
                        {
                            Vector3 temp = secondPosition;
                            secondPosition = firstPosition;
                            firstPosition = temp;
                        }

                        Vector3 vectorBetween = secondPosition - firstPosition;
                        Vector3 newForward = Vector3.Cross(vectorBetween, Vector3.up).normalized;
                        
                        float angle;
                        angle = Vector3.Angle(Vector3.forward, newForward);
                        if(firstPosition.z > secondPosition.z)
                            angle = -angle;

                        if(latestTransition != null)
                            StopCoroutine(latestTransition);

                        latestTransition = 
                            StartCoroutine(BarrierTransition(transform.GetChild(2), midpoint, angle, Mathf.Clamp(vectorBetween.magnitude, 1, 3)));
                    }
                    isNewBarrier = !isNewBarrier;
                }
            }
        }

        CopyBarrierTransformData(arenaProxyTransform.GetChild(0), arenaTransform.GetChild(0));
        CopyBarrierTransformData(arenaProxyTransform.GetChild(1), arenaTransform.GetChild(1));
        CopyBarrierTransformData(arenaProxyTransform.GetChild(2), arenaTransform.GetChild(2));
        CopyBarrierTransformData(arenaProxyTransform.GetChild(3), arenaTransform.GetChild(3));
    }
    
    // Used for network communication
    void FixedUpdate()
    {
        /* 
        if(ArenaController.IsReady && ArenaController.IsOpponentReady)
        {
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
        */
    }

    public void RefreshArena(ClientMessage clientMessage)
    {
        /* 
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
        */
    }
    public void OnBallCollision(string ballName, string hitName)
    {
        if(ballName == "ServerBallProxy" && hitName == "ClientWallProxy")
        {
            ServerScore++;
        }
        if(ballName == "ClientBallProxy" && hitName == "ServerWallProxy")
        {
            ClientScore++;
        }
    }

    private void CopyBarrierTransformData(Transform from, Transform to)
    {
        to.localPosition = new Vector3(from.localPosition.x, 0.5f, from.localPosition.y);
        to.localRotation= Quaternion.Euler(0, -from.eulerAngles.z, 0);
        to.localScale = new Vector3(from.localScale.x, from.localScale.z, from.localScale.y);
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
        serverBallController.ballBody.velocity = BallController.ServerStartVelocity;
        clientBallController.ballBody.velocity = BallController.ServerStartVelocity;
    }

    // Relaunch after scoring
    private void BallsRelaunch()
    {
        /* 
        TO DO reposition
        serverBallController.Direction = Vector3.forward;
        clientBallController.Direction = -Vector3.forward;
        serverBallController.Speed = 0.01f;
        clientBallController.Speed = 0.01f;
        */

        StartCoroutine(DelayedBallsLaunch());
    }

    // Neutralising random clicks
    private IEnumerator DelayedBarrierRefresh()
    {
        yield return new WaitForSeconds(1);
        isNewBarrier = true;
    }

    // Moving barrier according to two touch/mouse coordinates
    private IEnumerator BarrierTransition(Transform barrierTransform, Vector3 toPosition, float toAngle, float toScale)
    {
        Collider2D barrierCollider = barrierTransform.gameObject.GetComponent<Collider2D>();

        Vector3 startPosition = barrierTransform.localPosition;
        Quaternion startRotation = barrierTransform.localRotation;
        Vector3 startScale = barrierTransform.localScale;

        Vector3 projectedToPosition = new Vector3(toPosition.x, toPosition.z, 0);
        Quaternion projectedToRotation = Quaternion.Euler(0,0,toAngle);
        Vector3 projectedToScale = new Vector3(toScale, 0.1f, 1);

        float startTime = Time.time;
        float translationProgress = 0, rotationProgress = 0, scaleProgress = 0;

        float[] durations = new float[] {
            Vector3.Distance(startPosition, projectedToPosition)/9,
            Mathf.Abs(startRotation.eulerAngles.z - projectedToRotation.eulerAngles.z)/720,
            Mathf.Abs(startScale.x - projectedToScale.x)/4
        };

        float[] progresses = new float[3];

        int biggestDurationIndex = 0;
        for(int i = 1; i < 3; i++)
            if(durations[i] > durations[biggestDurationIndex])
                biggestDurationIndex = i;
        
        while(progresses[biggestDurationIndex] < 1)
        {
            for(int i = 0; i < 3; i++)
                progresses[i] = (Time.time - startTime)/durations[i];

            barrierTransform.localPosition = 
                Vector3.Lerp(startPosition, projectedToPosition, progresses[0]);

            barrierTransform.localRotation = 
                Quaternion.Lerp(startRotation, projectedToRotation, progresses[1]);

            Vector3 scaleLerp = 
                Vector3.Lerp(startScale, projectedToScale, progresses[2]);

            if (!float.IsNaN(scaleLerp.x) && !float.IsNaN(scaleLerp.y) && !float.IsNaN(scaleLerp.z))
            {
                barrierTransform.localScale = scaleLerp;
            }
            
            int contactsQuantity = barrierCollider.GetContacts(barrierContacts);
            bool isCollided = false;
            for(int i = 0; i < contactsQuantity; i++)
                if(barrierContacts[i].collider.gameObject.layer == 10)
                {
                    isCollided = true;
                    break;
                }
            if(isCollided)
                break;

            yield return null;
        }
        latestTransition = null;
        yield return null;
    }

    private IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(3);
        Destroy(NetworkController.Provider);
        SceneManager.LoadScene("NetworkMenu", LoadSceneMode.Single);
    }
}
