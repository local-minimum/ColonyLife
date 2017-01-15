using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ManipulationMode { BatchTransfer, Mutate, Kill};
public enum ManipulationEventType { None, Start, End, Instant};

public delegate void ManipulationEvent(ManipulationMode mode, ManipulationEventType modeType, Ray r, LayerMask layers);

public class CultureManipulator : MonoBehaviour {

    public static event ManipulationEvent OnManipulation;

    Camera microscopeCam;

    [SerializeField]
    LayerMask rayLayers;

    ManipulationMode mode = ManipulationMode.Kill;
    ManipulationEventType curState = ManipulationEventType.None;

    bool firstTime = true;

    [SerializeField]
    KeyCode KillKey = KeyCode.L;

    [SerializeField]
    KeyCode BatchKey = KeyCode.B;

    [SerializeField]
    KeyCode MutateKey = KeyCode.M;

    [SerializeField]
    float scrollSensitivity = 1f;

    void Start()
    {
        microscopeCam = GetComponent<Camera>();
    }

    void Update () {
        if (firstTime)
        {
            if (OnManipulation != null)
            {
                OnManipulation(mode, curState, new Ray(), rayLayers);
            }
            firstTime = false;
        }

        ManipulationMode nextMode;
        if (SwappedMode(out nextMode))
        {
            if (curState == ManipulationEventType.Start)
            {
                curState = ManipulationEventType.End;
                if (OnManipulation != null)
                {
                    OnManipulation(mode, curState, microscopeCam.ScreenPointToRay(Input.mousePosition), rayLayers);
                }
            }

            curState = ManipulationEventType.None;
            mode = nextMode;

            if (OnManipulation != null) {
                OnManipulation(mode, curState, microscopeCam.ScreenPointToRay(Input.mousePosition), rayLayers);
            }
        }

        bool eventing = false;
        if (Input.GetMouseButtonDown(0))
        {
            curState = mode == ManipulationMode.BatchTransfer ? ManipulationEventType.Instant : ManipulationEventType.Start;
            eventing = true;
        } else if (Input.GetMouseButtonUp(0) && mode != ManipulationMode.BatchTransfer)
        {
            curState = ManipulationEventType.End;
            eventing = true;
        }

        if (eventing && OnManipulation != null)
        {
            OnManipulation(mode, curState, microscopeCam.ScreenPointToRay(Input.mousePosition), rayLayers);
        }
    }

    bool SwappedMode(out ManipulationMode nextMode)
    {
        int scrollSwitch = Mathf.FloorToInt(Input.mouseScrollDelta.y / scrollSensitivity);
        if (scrollSwitch != 0)
        {
            int newMode = (int)mode + scrollSwitch;
            if (newMode < 0)
            {
                newMode += Mathf.RoundToInt(3f * Mathf.Ceil(-newMode / 3f));
            }
            newMode %= 3;
            nextMode = (ManipulationMode)newMode;
            return true;
        } else if (Input.GetKeyDown(KillKey))
        {
            nextMode = ManipulationMode.Kill;
            return true;
        } else if (Input.GetKeyDown(MutateKey))
        {
            nextMode = ManipulationMode.Mutate;
            return true;
        } else if (Input.GetKeyDown(BatchKey))
        {
            nextMode = ManipulationMode.BatchTransfer;
            return true;
        }

        nextMode = mode;
        return false;
    }
}
