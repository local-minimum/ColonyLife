using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManipulatorModeIcon : MonoBehaviour {

    [SerializeField]
    Sprite[] modeSprites;
    
    Image iconImage;

    [SerializeField]
    string swapTrigger;

    Animator anim;

    void Start()
    {
        iconImage = GetComponent<Image>();
        anim = GetComponent<Animator>();
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

    private void CultureManipulator_OnManipulation(ManipulationMode mode, ManipulationEventType modeType, Ray r, LayerMask layers)
    {
        if (modeType == ManipulationEventType.None)
        {
            iconImage.sprite = modeSprites[(int) mode];
            anim.SetTrigger(swapTrigger);
        }
    }
}
