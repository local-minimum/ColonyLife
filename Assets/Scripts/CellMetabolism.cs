using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CellMetabolism : MonoBehaviour {

    [SerializeField, Range(0, 1)]
    float motherSize = 0.75f;

    [SerializeField]
    int mitosisMutations = 5;

    [SerializeField]
    int senecence = 10;

    [SerializeField, Range(0, 1)]
    float daughterAge = 0.25f;

    int mutationEvents = 0;
    int age;

    [SerializeField, Range(0, 1)]
    float nutrientState = 0.5f;

    [SerializeField]
    float viewSizeMax = 0.5f;

    [SerializeField]
    float viewSizeMin = 0.1f;

    void Update()
    {
        nutrientState = Mathf.Clamp01(nutrientState + (Nutrient.GetCollidingNutrients(transform).Count() - 0.5f) * 0.05f * Time.deltaTime);
        transform.localScale = Vector3.one * Mathf.Lerp(viewSizeMin, viewSizeMax, nutrientState);
    }
}
