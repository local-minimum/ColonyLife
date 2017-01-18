using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CellCountUI : MonoBehaviour {

    [SerializeField]
    Text cellCount;

    [SerializeField]
    Text popSizeDoubleTime;

    [SerializeField]
    GraphSprite popSizeGraph;

    [SerializeField]
    Text cycle;

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
        cycles++;
        cycle.text = cycles.ToString();

        recording = false;
        StartCoroutine(Recorder(parentals.Count));
    }

    [SerializeField, Range(0, 1)]
    float experimentTimeSampleFreq = 1 / 3f;

    [SerializeField]
    int gtTimeDelta = 5;

    int cycles = 0;

    IEnumerator<WaitForSeconds> Recorder(int curCount) {

        //Getting out of previous recordings and ensuring all data needed is up to date;
        yield return new WaitForSeconds(0.1f);
        times.Clear();
        counts.Clear();
        int records = 0;
        float nextTime = 0 + experimentTimeSampleFreq;
        times.Add(0);
        counts.Add(curCount);
        popSizeGraph.Plot(times.ToArray(), counts.Select(e => (float) e).ToArray());
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
                popSizeGraph.SetData(times.ToArray(), counts.Select(e => (float)e).ToArray());
                records++;
                updateUI = true;
            }
            if (updateUI) {

                updateUI = false;

                cellCount.text = curCount.ToString();

                if (records >= gtTimeDelta)
                {
                    int prevSize = counts[records - gtTimeDelta];
                    float deltaTime = time - times[records - gtTimeDelta];

                    float sizeFactor = curCount / (float)prevSize;
                    float doublingTime = deltaTime * log2 / Mathf.Log(sizeFactor);

                    popSizeDoubleTime.text = doublingTime.ToString("0.000") + " h";
                }
            }
            yield return new WaitForSeconds(0.02f);
        }
	}
}
