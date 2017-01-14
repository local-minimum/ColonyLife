using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nutrient : MonoBehaviour {
    
    static List<Nutrient> nutrients = new List<Nutrient>();

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

    void OnDestroy()
    {
        nutrients.Remove(this);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        Bounce(beakerLocalPosition);
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
            direction.x = -Mathf.Abs(direction.x);
        } else if (localPos.x < -1f)
        {
            direction.x = Mathf.Abs(direction.x);
        }

        if (localPos.z > 1f)
        {
            direction.z = -Mathf.Abs(direction.z);
        } else if (localPos.z < -1f)
        {
            direction.z = Mathf.Abs(direction.z);
        }
    }
    
}
