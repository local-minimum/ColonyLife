using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MediaComponentComposition : MonoBehaviour {

    [SerializeField]
    bool topComponent = false;

    [SerializeField]
    float expandedSize = 160f;

    [SerializeField]
    float collapsedSize = 35f;

    [SerializeField]
    Slider AmountSlider;

    [SerializeField]
    Slider DiffusionSlider;

    [SerializeField]
    Slider ComplexitySlider;

    [SerializeField]
    Slider ToxicitySlider;

    [SerializeField]
    Slider NutritionSlider;

    [SerializeField]
    Button foldingButton;

    [SerializeField]
    Text componentName;

    [SerializeField]
    MediaComponentComposition nextComponent;

    bool isExpanded = false;

    RectMask2D mask;

    void Start()
    {
        mask = GetComponent<RectMask2D>();

        ChangedComponent();
        
        if (topComponent)
        {
            ShowContent();
        } else
        {
            CollapseContent();
        }
    }

    public void ShowContent()
    {
        RectTransform rt = transform as RectTransform;
        Vector2 sizeDelta = rt.sizeDelta;
        sizeDelta.y = expandedSize;
        rt.sizeDelta = sizeDelta;
        isExpanded = true;
        foldingButton.transform.localScale = new Vector3(1, -1, 1);
        
        SetNextComponent();
    }    

    public void CollapseContent()
    {
        RectTransform rt = transform as RectTransform;
        Vector2 sizeDelta = rt.sizeDelta;
        sizeDelta.y = collapsedSize;
        rt.sizeDelta = sizeDelta;
        isExpanded = false;
        foldingButton.transform.localScale = new Vector3(1, 1, 1);
        
        SetNextComponent();
    }

    void SetNextComponent()
    {

        if (nextComponent == null)
        {
            return;
        }

        RectTransform rt = transform as RectTransform;
        RectTransform nextRt = nextComponent.transform as RectTransform;

        Vector3 pos = rt.localPosition;
        pos.y -= 5 + (isExpanded ? expandedSize : collapsedSize);

        nextRt.localPosition = pos;

        nextComponent.SetNextComponent();
    }

    public void ToggleShowCollapse()
    {
        if (isExpanded)
        {
            CollapseContent();
        } else
        {
            ShowContent();
        }
    }

    public void ChangedComponent()
    {
        componentName.text = GenerateName();
    }

    string GenerateName()
    {
        float amount = AmountSlider.value;
        float diffusion = DiffusionSlider.value;
        float complexity = ComplexitySlider.value;
        float toxicity = ToxicitySlider.value;
        float nutrition = NutritionSlider.value;

        if (nutrition == 0 && toxicity == 0)
        {
            return "Water";
        }

        return "Glucose";
    }
}
