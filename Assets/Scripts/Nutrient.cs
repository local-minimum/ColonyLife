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

    void Awake()
    {
        nutrients.Add(this);
    }

    void OnDestroy()
    {
        nutrients.Remove(this);
    }
}
