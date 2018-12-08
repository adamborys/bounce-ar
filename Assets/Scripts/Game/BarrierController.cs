using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierController : MonoBehaviour
{
    bool isNewBarrier = true;
    Vector3[] positions;
	GameObject barrier;

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
            LayerMask mask = LayerMask.GetMask("Default");
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
            {
                if (isNewBarrier)
                    positions[0] = new Vector3(hit.point.x, 0.5f, hit.point.z);
				else
				{
					positions[1] = new Vector3(hit.point.x, 0.5f, hit.point.z);
				
					Vector3 midpoint = (positions[1] - positions[0]) / 2 + positions[0];

					Destroy(barrier);
					barrier = GameObject.CreatePrimitive(PrimitiveType.Cube);
					barrier.name = "Barrier";
					barrier.transform.position = midpoint;
					barrier.transform.LookAt(positions[1]);
					barrier.transform.localScale = new Vector3(0.02f, 0.5f, 3);
				}
                isNewBarrier = !isNewBarrier;
            }
        }
    }
}
