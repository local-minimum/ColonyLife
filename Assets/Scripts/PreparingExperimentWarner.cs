using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreparingExperimentWarner : MonoBehaviour {

    [SerializeField]
    GameObject target;

    void OnEnable()
    {
        Culture.OnNewBatch += Culture_OnNewBatch;
    }

    void OnDisable()
    {
        Culture.OnNewBatch -= Culture_OnNewBatch;
    }

    void OnDestroy()
    {
        Culture.OnNewBatch -= Culture_OnNewBatch;

    }

    private void Culture_OnNewBatch(List<CellMetabolism> parentals)
    {
        target.SetActive(false);
    }
}
