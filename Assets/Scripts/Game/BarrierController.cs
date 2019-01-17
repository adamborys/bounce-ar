using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierController : MonoBehaviour
{
    private void OnCollisionEnter(Collision col)
    {
        if(NetworkController.IsServer)
        {
            transform.GetComponentInParent<GameServerController>().OnBarrierCollision(gameObject, col.gameObject);
        }
    }
}
