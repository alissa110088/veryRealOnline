using System;
using UnityEngine;

public static class ActionManager 
{
    public static Action<GameObject, Vector3, Camera> spawnUi;
    public static Action despawnUi;
    public static Action grab;  
}
