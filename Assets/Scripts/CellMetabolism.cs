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

    static List<CellMetabolism> _population = new List<CellMetabolism>();

    public static IEnumerable<CellMetabolism> SamplePopulation(int count)
    {
        for (int i = 0; i < count; i++)
        {
            CellMetabolism cell = _population[Random.Range(0, _population.Count)];
            cell.enabled = false;
            _population.Remove(cell);
            _populationSize--;
            yield return cell;
        }
    }

    public static IEnumerable<bool[]> Genomes()
    {
        return _population.Select(e => e.genome);
    }

    public static void WipePopulation()
    {
        for (int i = 0, l = _population.Count; i < l; i++)
        {
            CellMetabolism cell = _population[i];
            cell.enabled = false;
            Destroy(cell.gameObject);

        }

        //Destroy clears _population and decreases the popcount itself
    }

    [HideInInspector]
    public Culture culture;

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

    public bool[] Genome {
        get
        {
            return genome;
        }
    }

    void Start()
    {
        if (hasCalculatedNutrientValue == null)
        {
            CreateNutrientLookup();
        }
        UpdateSize();        
    }

    void CreateNutrientLookup()
    {
        hasCalculatedNutrientValue = new bool[Media.NutrientTypes];
        nutrientValue = new float[Media.NutrientTypes];
        
    }

    public void CopyGenome(CellMetabolism other, bool mitosisMutate)
    {
        genomeSize = other.genomeSize;
        if (genome == null || genome.Length != genomeSize)
        {
            genome = new bool[genomeSize];
        }
        other.genome.CopyTo(genome, 0);

        if (hasCalculatedNutrientValue == null)
        {
            CreateNutrientLookup();
        }

        if (mitosisMutate)
        {
            MitosisMutate();
        }
    }

    public void ResetAge()
    {
        age = 0;
    }

    public void SetNutrientState(float value)
    {
        nutrientState = Mathf.Min(1, value);
    }

    void OnEnable()
    {
        if (!_population.Contains(this))
        {
            _populationSize++;
            _population.Add(this);
        }
    }

    void OnDestroy()
    {
        if (_population.Contains(this))
        {
            _population.Remove(this);
            _populationSize--;
        }
    }

    public void CreateGenome()
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
        ClearNutrientLookup();
    }

    public void Mutate()
    {
        int pos = Random.Range(0, genomeSize);
        genome[pos] = !genome[pos];
        ClearNutrientLookup();
        mutationEvents++;
    }

    void ClearNutrientLookup()
    {        
        for (int i=0, l=Media.NutrientTypes; i< l; i++)
        {
            hasCalculatedNutrientValue[i] = false;
        }
    }       

    float currentDeltaNutrition = 0f;
    int frame = 0;

    void Update()
    {
        if (frame == 7)
        {
            currentDeltaNutrition = Nutrient.GetCollidingNutritionalValue(transform, NutrientValue) * metabolismFactor;
            frame = 0;
        } else
        {
            frame++;
        }
        nutrientState += currentDeltaNutrition * Time.deltaTime;
        UpdateSize();

        if (nutrientState >= 1f)
        {
            Clone();
        }
    }

    void UpdateSize()
    {       
        transform.localScale = Vector3.one * Mathf.Lerp(viewSizeMin, viewSizeMax, nutrientState);
    }

    bool[] hasCalculatedNutrientValue;
    float[] nutrientValue;

    float NutrientValue(Nutrient nutrient)
    {
        if (hasCalculatedNutrientValue[nutrient.nutrientTypeIndex])
        {
            return nutrientValue[nutrient.nutrientTypeIndex];
        } else
        {
            nutrientValue[nutrient.nutrientTypeIndex] = nutrient.CalculateNutrientEffect(genomeSize, genome);
            hasCalculatedNutrientValue[nutrient.nutrientTypeIndex] = true;
            return nutrientValue[nutrient.nutrientTypeIndex];
        }
    }

    void Clone()
    {
        //Add a new cell cycle to self
        age++;
        nutrientState = 1f;

        if (_populationSize < culture.populationMaxSize)
        {
            //Create daughter
            Vector3 position = transform.TransformPoint(new Vector3(Random.Range(-0.5f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized);
            GameObject daughter = Instantiate(gameObject, position, Quaternion.identity, transform.parent);

            //Set daughter params
            CellMetabolism daughterMetabolism = daughter.GetComponent<CellMetabolism>();
            daughterMetabolism.age = Mathf.FloorToInt(age * daughterAge);
            daughterMetabolism.nutrientState = nutrientState * (1 - motherSize);
            daughterMetabolism.CopyGenome(this, true);
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

    int burnCount = 0;
    int maxBurn = 7;

    public void Burn()
    {
        burnCount++;
        if (burnCount > maxBurn)
        {
            Destroy(gameObject);
        }
    }
}
