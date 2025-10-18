using System;
using UnityEngine;

public static class ActionManager 
{
    public static Action<GameObject, Vector3, Camera> spawnUi;
    public static Action<GameObject> despawnUi;
    public static Action grab;
    public static Action release;
    public static Action<PlayerNetwork> addPlayer;
    public static Action activatePlayer;
}
