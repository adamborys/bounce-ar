using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
					Destroy(Barrier);
                    positions[0] = transform.InverseTransformPoint(hit.point);
                }
				else
				{
					positions[1] = transform.InverseTransformPoint(hit.point);
                    positions[0].y = positions[1].y = 0.0035f;
					Vector3 midpoint = (positions[1] - positions[0]) / 2 + positions[0];

					Barrier = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Barrier.transform.SetParent(transform, false);
					Barrier.name = "Barrier";
					Barrier.transform.localPosition = midpoint;
                    float barrierLength = Vector3.Distance(positions[0], positions[1]);
                    Barrier.transform.localScale = 
                        new Vector3(0.0002f, 0.005f, Mathf.Clamp(barrierLength, 0.005f, 0.03f));
					Barrier.transform.LookAt(transform.TransformPoint(positions[1]), transform.up);
				}
                isNewBarrier = !isNewBarrier;
            }
        }
    }
}
