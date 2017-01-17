using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimeConverter : MonoBehaviour {

    static float startGameTime;
    
    static float gameSecondsToHours = 0.25f;

    public static float experimentTime
    {
        get
        {
            return (Time.timeSinceLevelLoad - startGameTime) * gameSecondsToHours;
        }
    }

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
        started = true;
        startGameTime = Time.timeSinceLevelLoad;
    }

    bool started = false;

    [SerializeField]
    Text clockUI;

    void Update()
    {
        if (started && clockUI)
        {
            clockUI.text = experimentTime.ToString("0.00") + " h";
        }
    }
}
