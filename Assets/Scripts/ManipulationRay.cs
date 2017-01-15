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

    Vector3 startFocus;
    Vector3 inputStartPos;

    private void CultureManipulator_OnManipulation(ManipulationMode mode, ManipulationEventType modeType, Ray r, LayerMask layers)
    {
        if (mode == activeMode)
        {
            if (modeType == ManipulationEventType.Start)
            {                
                startFocus = r.GetPoint(rayFocusDepth);
                inputStartPos = Input.mousePosition;
                manipulationRay.SetPosition(1, transform.InverseTransformPoint(startFocus));
                manipulationRay.enabled = true;
                Cursor.visible = false;

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
            startFocus += new Vector3(delta.x, 0, delta.y) * moveScale + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1, 1f)) * moveNoise;
            manipulationRay.SetPosition(1, transform.InverseTransformPoint(startFocus));
        }
	}
}
