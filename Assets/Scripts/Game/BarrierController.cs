using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierController : MonoBehaviour
{
    bool isNewBarrier = true;
    Vector3[] positions;

    void Start()
    {
        positions = new Vector3[2];
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = GameStartController.debugCamera
						.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (isNewBarrier)
                    positions[0] = new Vector3(hit.point.x, 0.005f, hit.point.z);
				else
					positions[1] = new Vector3(hit.point.x, 0.005f, hit.point.z);
                isNewBarrier = !isNewBarrier;
				if(isNewBarrier)
				{
					Debug.Log(positions[0]);
					Debug.Log(positions[1]);
				}
            }
        }
    }
}
