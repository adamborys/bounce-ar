using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierController : MonoBehaviour
{
	public static GameObject Barrier;
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
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
            LayerMask mask = LayerMask.GetMask("Default");
            // Ignoring other layers than "Default"
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
            {
                if (isNewBarrier)
                {
					Destroy(Barrier);
                    positions[0] = transform.InverseTransformPoint(hit.point);
                }
				else
				{
					positions[1] = transform.InverseTransformPoint(hit.point);
					Vector3 midpoint = (positions[1] - positions[0]) / 2 + positions[0];
                    midpoint.y = positions[1].y = 0.0035f;

					Barrier = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Barrier.transform.SetParent(transform, false);
					Barrier.name = "Barrier";
					Barrier.transform.localPosition = midpoint;
                    Barrier.transform.localScale = 
                        new Vector3(0.002f, 0.05f, 0.3f) / transform.localScale.x;
					Barrier.transform.LookAt(transform.TransformPoint(positions[1]), transform.up);
				}
                isNewBarrier = !isNewBarrier;
            }
        }
    }
}
