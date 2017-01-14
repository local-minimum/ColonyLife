using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CellMetabolism : MonoBehaviour {

    static int _populationSize = 0;

    public static int populationSize
    {
        get
        {
            return _populationSize;
        }
    }

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

    [SerializeField]
    int genomeSize = 64;

    bool[] genome;

    void Start()
    {
        _populationSize += 1;

        if (genome == null)
        {
            CreateGenome();    
        } else
        {
            MitosisMutate();
        }
    }

    void CreateGenome()
    {
        genome = new bool[genomeSize];
        for (int i=0; i<genomeSize; i++)
        {
            genome[i] = Random.value < 0.5f;
        }
    }

    void MitosisMutate()
    {
        mutationEvents++;
        for (int i=0; i<mitosisMutations; i++)
        {
            int pos = Random.Range(0, genomeSize);
            genome[pos] = !genome[pos];
        }
        nutrientLookup.Clear();
    }

    public void Mutate()
    {
        int pos = Random.Range(0, genomeSize);
        genome[pos] = !genome[pos];
        nutrientLookup.Clear();
        mutationEvents++;
    }

    void Update()
    {
        nutrientState = Mathf.Clamp01(nutrientState + Nutrient.GetCollidingNutrients(transform).Select(e => NutrientValue(e)).Sum() * metabolismFactor * Time.deltaTime);
        transform.localScale = Vector3.one * Mathf.Lerp(viewSizeMin, viewSizeMax, nutrientState);
        if (nutrientState >= 1f)
        {
            Clone();
        }
    }

    Dictionary<bool[], float> nutrientLookup = new Dictionary<bool[], float>();

    float NutrientValue(Nutrient nutrient)
    {
        if (!nutrientLookup.Keys.Contains(nutrient.nutrientGenome)) {
            nutrientLookup[nutrient.nutrientGenome] = CalculateNutrientEffect(nutrient);
        }

        return nutrientLookup[nutrient.nutrientGenome];
    }

    float CalculateNutrientEffect(Nutrient nutrient)
    {

        int maxGain = 0;
        int maxLoss = 0;

        for (int offset = 0, endOffset = genomeSize - nutrient.nutrientSize; offset < endOffset; offset++)
        {
            int gains = 0;
            for (int genPos= offset, endJ = offset + nutrient.nutrientSize, nutrPos=0; genPos< endJ; genPos++, nutrPos++)
            {
                if (nutrient.nutrientGenome[nutrPos] == genome[genPos])
                {
                    gains++;
                }
            }
            int loss = nutrient.nutrientSize - gains;
            if (gains > maxGain)
            {
                maxGain = gains;
            }
            if (loss > maxLoss)
            {
                maxLoss = loss;
            }
        }

        if (maxGain == nutrient.nutrientSize)
        {
            Debug.Log("Optimum Reached");
        }

        if (maxGain >= maxLoss)
        {
            return maxGain;
        } else
        {
            return maxLoss;
        }
        
    }

    void Clone()
    {
        //Add a new cell cycle to self
        age++;

        if (_populationSize < 3000)
        {
            //Create daughter
            Vector3 position = transform.TransformPoint(new Vector3(Random.Range(-0.5f, 1), Random.Range(-1, 1), Random.Range(-1, 1)).normalized);
            GameObject daughter = Instantiate(gameObject, position, Quaternion.identity, transform.parent);

            //Set daughter params
            CellMetabolism daughterMetabolism = daughter.GetComponent<CellMetabolism>();
            daughterMetabolism.age = Mathf.FloorToInt(age * daughterAge);
            daughterMetabolism.nutrientState = nutrientState * (1 - motherSize);
        }

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
