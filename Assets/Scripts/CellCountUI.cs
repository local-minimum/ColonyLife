using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellCountUI : MonoBehaviour {

    [SerializeField]
    Text cellCount;

    [SerializeField]
    Text popSizeDoubleTime;

    float log2 = Mathf.Log(2f);

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

    bool recording = false;
    List<float> times = new List<float>();
    List<int> counts = new List<int>();

    private void Culture_OnNewBatch(List<CellMetabolism> parentals)
    {
        recording = false;
        StartCoroutine(Recorder(parentals.Count));
    }

    [SerializeField, Range(0, 1)]
    float experimentTimeSampleFreq = 1 / 3f;
    
    IEnumerator<WaitForSeconds> Recorder(int curCount) {

        //Getting out of previous recordings and ensuring all data needed is up to date;
        yield return new WaitForSeconds(0.1f);
        times.Clear();
        counts.Clear();
        int records = 0;
        float nextTime = 0 + experimentTimeSampleFreq;
        times.Add(0);
        counts.Add(curCount);
        recording = true;

        bool updateUI = true;

        while (recording)
        {
            float time = GameTimeConverter.experimentTime;
            if (time > nextTime)
            {
                times.Add(time);
                nextTime = time + experimentTimeSampleFreq;

                curCount = CellMetabolism.populationSize;
                counts.Add(curCount);

                records++;
                updateUI = true;
            }
            if (updateUI) {

                updateUI = false;

                cellCount.text = curCount.ToString();

                if (records > 1)
                {
                    int prevSize = counts[records - 2];
                    float deltaTime = time - times[records - 2];

                    float sizeFactor = curCount / (float)prevSize;
                    float doublingTime = deltaTime * log2 / Mathf.Log(sizeFactor);

                    popSizeDoubleTime.text = doublingTime.ToString("0.000") + " h";
                }
            }
            yield return new WaitForSeconds(0.02f);
        }
	}
}
