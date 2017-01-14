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

    [SerializeField]
    float metabolismFactor = 0.05f;

    void Update()
    {
        nutrientState = Mathf.Clamp01(nutrientState + (Nutrient.GetCollidingNutrients(transform).Count() - 0.5f) * metabolismFactor * Time.deltaTime);
        transform.localScale = Vector3.one * Mathf.Lerp(viewSizeMin, viewSizeMax, nutrientState);
        if (nutrientState >= 1f)
        {
            Clone();
        }
    }

    void Clone()
    {
        //Add a new cell cycle to self
        age++;

        //Create daughter
        Vector3 position = transform.TransformPoint(new Vector3(Random.Range(-0.5f, 1), Random.Range(-1, 1), Random.Range(-1, 1)).normalized);
        GameObject daughter = Instantiate(gameObject, position, Quaternion.identity, transform.parent);

        //Set daughter params
        CellMetabolism daughterMetabolism = daughter.GetComponent<CellMetabolism>();
        daughterMetabolism.age = Mathf.FloorToInt(age * daughterAge);
        daughterMetabolism.nutrientState = nutrientState * (1 - motherSize);

        //Shelf mother if too many kids
        if (age >= senecence)
        {
            Kill();
        }
        else {
            nutrientState *= motherSize;
        }
    }

    void Kill()
    {
        this.enabled = false;
    }
}
