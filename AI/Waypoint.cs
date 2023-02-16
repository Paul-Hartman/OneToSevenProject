using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Waypoint
{
    public enum ActionsPatrol { GO = 0, STAY }
    public ActionsPatrol Action;
    public GameObject Target;
    public Vector3 Position;
    public float Duration;
    
}
