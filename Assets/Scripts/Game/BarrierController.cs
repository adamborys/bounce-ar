using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierController : MonoBehaviour
{
    private void OnCollisionEnter(Collision col)
    {
        transform.GetComponentInParent<GameServerController>().OnBarrierCollision(gameObject, col.gameObject);
    }
}
