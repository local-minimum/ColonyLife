using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellCountUI : MonoBehaviour {

    [SerializeField]
    Text cellCount;

    [SerializeField]
    Text popSizeDoubleTime;

    [SerializeField]
    float doublingTimeSampleFreq = 3f;

    float sampleTime = 0f;

    int prevSize;

    float log2 = Mathf.Log(2f);

	// Update is called once per frame
	void Update () {

        cellCount.text = CellMetabolism.populationSize.ToString();

        if (Time.timeSinceLevelLoad > sampleTime + doublingTimeSampleFreq)
        {
            int deltaSize = CellMetabolism.populationSize - prevSize;
            float deltaTime = Time.timeSinceLevelLoad - sampleTime;
            sampleTime = Time.timeSinceLevelLoad;
            float sizeFactor = (prevSize + deltaSize) / (float)prevSize;
            prevSize += deltaSize;

            float doublingTime = deltaTime * log2 / Mathf.Log(sizeFactor);

            popSizeDoubleTime.text = doublingTime.ToString("0.000");
        }
	}
}
