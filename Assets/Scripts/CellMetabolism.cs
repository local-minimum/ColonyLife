using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CellMetabolism : MonoBehaviour {

    public static int populationSize = 0;

    static List<CellMetabolism> _population = new List<CellMetabolism>();
    static List<CellMetabolism> _InactivePopulation = new List<CellMetabolism>();

    public static IEnumerable<CellMetabolism> SamplePopulation(int count)
    {
        for (int i = 0; i < count; i++)
        {
            CellMetabolism cell = _population[Random.Range(0, _population.Count)];
            cell.enabled = false;
            _population.Remove(cell);
            _InactivePopulation.Add(cell);
            populationSize--;
            yield return cell;
        }
    }

    public static void AddToPopulation(CellMetabolism cell)
    {
        _population.Add(cell);
        populationSize++;
        if (_InactivePopulation.Contains(cell))
        {
            _InactivePopulation.Remove(cell);
        }
    }

    public static List<CellMetabolism> GetInactive(int count)
    {
        List<CellMetabolism> inactive = new List<CellMetabolism>();
        for (int i=0; i<count; i++)
        {
            CellMetabolism cell = _InactivePopulation[0];
            inactive.Add(cell);
            _InactivePopulation.RemoveAt(0);
        }

        return inactive;
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
            _InactivePopulation.Add(cell);
            cell.enabled = false;
            cell.gameObject.SetActive(false);
        }
        _population.Clear();
        populationSize = 0;
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

    public void SetToInactive()
    {
        if (_population.Contains(this))
        {
            _population.Remove(this);
            populationSize = Mathf.Max(0, populationSize - 1);
        }
        if (!_InactivePopulation.Contains(this))
        {
            _InactivePopulation.Add(this);
            enabled = false;
            gameObject.SetActive(false);
        }
    }

    public void SetToActive()
    {
        if (_InactivePopulation.Contains(this))
        {
            _InactivePopulation.Remove(this);
        }
        if (!_population.Contains(this))
        {
            _population.Add(this);
            populationSize++;
        }
    }

    void Awake()
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
        ClearNutrientLookup();
    }

    public void SetNutrientState(float value)
    {
        nutrientState = Mathf.Min(1, value);
        requireNutrientLookup = true;
    }

    void OnDestroy()
    {
        if (_population.Contains(this))
        {
            _population.Remove(this);            
        }
        if (_InactivePopulation.Contains(this))
        {
            _InactivePopulation.Remove(this);
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
    bool requireNutrientLookup = false;

    void Update()
    {
        if (requireNutrientLookup || frame == 7)
        {
            currentDeltaNutrition = Nutrient.GetCollidingNutritionalValue(transform, NutrientValue) * metabolismFactor;
            frame = 0;
            requireNutrientLookup = false;

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
        return nutrient.CalculateNutrientEffect(genomeSize, genome);
        /*
        if (hasCalculatedNutrientValue[nutrient.nutrientTypeIndex])
        {
            return nutrientValue[nutrient.nutrientTypeIndex];
        } else
        {
            nutrientValue[nutrient.nutrientTypeIndex] = nutrient.CalculateNutrientEffect(genomeSize, genome);
            hasCalculatedNutrientValue[nutrient.nutrientTypeIndex] = true;
            return nutrientValue[nutrient.nutrientTypeIndex];
        }*/
    }

    void Clone()
    {
        //Add a new cell cycle to self
        age++;
        nutrientState = 1f;

        if (populationSize < culture.populationMaxSize)
        {
            //Create daughter
            Vector3 position = transform.TransformPoint(new Vector3(Random.Range(-1f, 1f), Random.Range(-.25f, 1f), Random.Range(-1f, 1f)).normalized);

            CellMetabolism daughterMetabolism = _InactivePopulation[0];
            daughterMetabolism.SetToActive();
            daughterMetabolism.transform.position = position;
            
            // GameObject daughter = Instantiate(gameObject, position, Quaternion.identity, transform.parent);

            //Set daughter params
            daughterMetabolism.age = Mathf.FloorToInt(age * daughterAge);
            daughterMetabolism.nutrientState = nutrientState * (1 - motherSize);
            daughterMetabolism.CopyGenome(this, true);

            daughterMetabolism.gameObject.SetActive(true);
            daughterMetabolism.enabled = true;
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
            SetToInactive();            
        }
    }
}
