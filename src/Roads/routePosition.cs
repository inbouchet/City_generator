using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct routePosition
{
    public Vector3 position, direction; 
    public float length;

    public routePosition(Vector3 position, Vector3 direction, float length)
    {
        this.position = position;
        this.direction = direction;
        this.length = length;
    }
}
