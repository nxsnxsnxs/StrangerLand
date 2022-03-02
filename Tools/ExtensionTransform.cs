using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionTransform
{
    public static void PlanerLookAt(this Transform transform, Vector3 point)
    {
        point.y = transform.position.y;
        transform.LookAt(point);
    }
}
