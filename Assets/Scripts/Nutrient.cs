using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nutrient : MonoBehaviour {
    
    static List<Nutrient> nutrients = new List<Nutrient>();
    Vector3 transformPos;

    public static IEnumerable<Nutrient> GetCollidingNutrients(Transform t)
    {
        float r2 = PlanarPhysics.ScaleToRadiusSq(t);
        Vector3 tPos = t.position;

        for (int i=0, l=nutrients.Count; i< l; i++)
        {
            if (PlanarPhysics.InsideSphereXZ(tPos, r2, nutrients[i].transform.position))
            {
                yield return nutrients[i];
            }
        }
    }

    public static float GetCollidingNutritionalValue(Transform t, System.Func<Nutrient, float> func)
    {
        //float r2 = PlanarPhysics.ScaleToRadiusSq(t);
        Vector3 tPos = t.position;
        float ret = 0;
        for (int i = 0, l = nutrients.Count; i < l; i++)
        {
            if (PlanarPhysics.InsideBlockXZ(tPos, 0.25f, nutrients[i].transformPos))
            {
                ret += func(nutrients[i]);
            }
        }
        return ret;
    }

    public static int Count {
        get
        {
            return nutrients.Count;
        }
    }

   
    public Transform beaker;

    Vector3 direction;

    [SerializeField]
    float speed = 0.1f;
    
    public int nutrientSize = 16;
    
    [HideInInspector]
    public bool[] nutrientGenome;

    [SerializeField]
    float nutrientPositiveFactor = 1f;

    [SerializeField]
    float nutrientDetrimentalFactor = 1f;

    //10 seconds of full pop size
    public float nutrientDepletionCapacity = 3500 * 10;

    public float nutrientUsage = 0;

    [HideInInspector]
    public int nutrientTypeIndex = -1;

    [SerializeField, Range(0, .5f)]
    float bounceNoise = 0.05f;

    public static bool[] CreateNutrientGenome(int nutrientSize)
    {
        bool[] nutrientGenome = new bool[nutrientSize];

        for (int i = 0; i < nutrientSize; i++)
        {
            nutrientGenome[i] = Random.value < 0.5f;
        }

        return nutrientGenome;
    }

    void Awake()
    {
        nutrients.Add(this);

        if (nutrientGenome == null || nutrientGenome.Length != nutrientSize)
        {
            CreateNutrient();
        }

        direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }    

    void CreateNutrient()
    {
        nutrientGenome = CreateNutrientGenome(nutrientSize);
    }

    public void SetNutrientGenome(bool[] nutrientGenome)
    {
        this.nutrientGenome = nutrientGenome;
        nutrientSize = nutrientGenome.Length;      
    }

    public void SetEffects(float positive, float detrimental)
    {
        nutrientPositiveFactor = positive;
        nutrientDetrimentalFactor = detrimental;
    }

    void OnDestroy()
    {
        nutrients.Remove(this);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;        
        Bounce(beakerLocalPosition);
        transformPos = transform.position;
        nutrientUsage += Time.deltaTime * CellMetabolism.populationSize;
    }

    Vector3 beakerLocalPosition
    {
        get
        {
            return beaker.InverseTransformPoint(transform.position) * 0.33f;
        }
    }

    void Bounce(Vector3 localPos)
    {

        if (localPos.x > 1f)
        {
            direction = new Vector3(-Mathf.Abs(direction.x) * (1 + Random.Range(-bounceNoise, bounceNoise)), 0, direction.z).normalized;
        } else if (localPos.x < -1f)
        {
            direction = new Vector3(Mathf.Abs(direction.x) * (1 + Random.Range(-bounceNoise, bounceNoise)), 0, direction.z).normalized;
        }

        if (localPos.z > 1f)
        {
            direction = new Vector3(direction.x, 0 , -Mathf.Abs(direction.z) * (1 + Random.Range(-bounceNoise, bounceNoise))).normalized;
        } else if (localPos.z < -1f)
        {
            direction = new Vector3(direction.x, 0, Mathf.Abs(direction.z) * (1 + Random.Range(-bounceNoise, bounceNoise))).normalized;
        }
    }

    public float CalculateNutrientEffect(int genomeSize, bool[] genome)
    {

        float maxGain = 0;
        float maxLoss = 0;

        for (int offset = 0, endOffset = genomeSize - nutrientSize; offset < endOffset; offset++)
        {
            int gains = 0;
            for (int genPos = offset, endJ = offset + nutrientSize, nutrPos = 0; genPos < endJ; genPos++, nutrPos++)
            {
                if (nutrientGenome[nutrPos] == genome[genPos])
                {
                    gains++;
                }
            }
            int loss = nutrientSize - gains;
            if (gains > maxGain)
            {
                maxGain = gains;
            }
            if (loss > maxLoss)
            {
                maxLoss = loss;
            }
        }

        if (maxGain >= nutrientSize)
        {
            Debug.Log("Optimum Reached");
        }

        maxGain = Mathf.Pow(maxGain / nutrientSize, 2) * nutrientPositiveFactor * (Mathf.Max(0, nutrientDepletionCapacity - nutrientUsage) / nutrientDepletionCapacity);
        maxLoss = Mathf.Pow(maxLoss / nutrientSize, 2) * nutrientDetrimentalFactor;

        if (maxGain > maxLoss)
        {
            return maxGain;
        }
        else
        {
            return -maxLoss;
        }

    }
}
