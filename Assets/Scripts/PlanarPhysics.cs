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
        return (Mathf.Pow(position.x - sphere.x, 2f) + Mathf.Pow(position.z - sphere.z, 2f)) < r2;
    }

    public static bool InsideBlockXZ(Vector3 taxi, float blockSize, Vector3 block)
    {
        return Mathf.Abs(taxi.x - block.x) < blockSize && Mathf.Abs(taxi.z - block.z) < blockSize;
    }
}
