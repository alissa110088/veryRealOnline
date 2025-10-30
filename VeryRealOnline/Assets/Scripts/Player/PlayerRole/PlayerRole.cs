using System.Collections.Generic;
using UnityEngine;

public class PlayerRole : MonoBehaviour
{
    private EnumPlayerState playerRole;

    [SerializeField] private Hider hiderScript;
    [SerializeField] private Seeker seekerScript;

    private void OnEnable()
    {
        ActionManager.GivePlayerRole += GiveRole;
    }

    private void OnDisable()
    {
        ActionManager.GivePlayerRole -= GiveRole;
    }

    private void GiveRole(EnumPlayerState pState, GameObject pPlayer)
    {
        if (pPlayer != gameObject)
            return;
        playerRole = pState;
        AttributeRole();
    }
    private void AttributeRole()
    {
        if(playerRole == EnumPlayerState.seeker)
        {
            seekerScript.enabled = true;
            gameObject.tag = "seeker";
        }
        else if(playerRole == EnumPlayerState.hider)
        {
            hiderScript.enabled = true;
            gameObject.tag = "hider";
        }
    }
}
