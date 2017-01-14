using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlanarPhysics {


    public static float ScaleToRadiusSq(Transform t)
    {
        Vector3 scale = t.lossyScale;
        return Mathf.Pow((scale.x + scale.z) / 4f, 2f);
    }

    public static bool InsideSphereXZ(Transform sphere, Vector3 position)
    {
        float r2 = ScaleToRadiusSq(sphere);
        Vector3 spherePosition = sphere.position;
        return InsideSphereXZ(spherePosition, r2, position);
    }

    public static bool InsideSphereXZ(Vector3 sphere, float r2, Vector3 position)
    {        
        return (Mathf.Pow(position.x - sphere.x, 2) + Mathf.Pow(position.z - sphere.z, 2)) < r2;
    }
}
