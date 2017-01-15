using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipulationRay : MonoBehaviour {

    [SerializeField]
    ManipulationMode activeMode;

    LineRenderer manipulationRay;

    [SerializeField]
    float rayFocusDepth = 10f;

    [SerializeField]
    float moveNoise = 0.001f;

    [SerializeField]
    float moveScale = 0.4f;

    void Start () {
        manipulationRay = GetComponent<LineRenderer>();	
	}

    void OnEnable()
    {
        CultureManipulator.OnManipulation += CultureManipulator_OnManipulation;
    }

    void OnDisable()
    {
        CultureManipulator.OnManipulation -= CultureManipulator_OnManipulation;
    }

    void OnDestroy()
    {
        CultureManipulator.OnManipulation -= CultureManipulator_OnManipulation;
    }    

    Vector3 focusPoint;
    Vector3 inputStartPos;
    LayerMask mask;

    private void CultureManipulator_OnManipulation(ManipulationMode mode, ManipulationEventType modeType, Ray r, LayerMask layers)
    {
        if (mode == activeMode)
        {
            if (modeType == ManipulationEventType.Start)
            {                
                focusPoint = r.GetPoint(rayFocusDepth);
                inputStartPos = Input.mousePosition;
                manipulationRay.SetPosition(1, transform.InverseTransformPoint(focusPoint));
                manipulationRay.enabled = true;
                Cursor.visible = false;
                mask = layers;

            } else
            {                
                manipulationRay.enabled = false;
                Cursor.visible = true;
            }
        }
    }

    void Update () {
		if (manipulationRay.enabled)
        {
            Vector2 delta = (Input.mousePosition - inputStartPos);
            focusPoint += new Vector3(delta.x, 0, delta.y) * moveScale + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1, 1f)) * moveNoise;
            manipulationRay.SetPosition(1, transform.InverseTransformPoint(focusPoint));
            Ray r = new Ray(transform.position, (focusPoint - transform.position).normalized);
            RaycastHit[] hits = Physics.RaycastAll(r, rayFocusDepth, mask);
            for (int i=0; i<hits.Length; i++)
            {
                CellMetabolism cell = hits[i].transform.GetComponent<CellMetabolism>();
                if (cell)
                {
                    cell.Burn();
                }
            }
        }
	}
}
