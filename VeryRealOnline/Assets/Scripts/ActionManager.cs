using System;
using UnityEngine;

public static class ActionManager 
{
    public static Action<GameObject, Vector3, Camera> spawnUi;
    public static Action<GameObject> despawnUi;
    public static Action<GameObject> grab;
    public static Action release;
    public static Action<PlayerNetwork> addPlayer;
    public static Action activatePlayer;
    public static Action<EnumPlayerState, GameObject, Vector3> GivePlayerRole;
    public static Action onSeekerWin;
    public static Action onHiderWin;
    public static Action ActivateMovement;
    public static Action DeactivateMovement;
}
