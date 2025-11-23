using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerRole : NetworkBehaviour
{
    private EnumPlayerState playerRole;
    private string hiderText = "hider";
    private string seekerText = "seeker";


    [SerializeField] private Hider hiderScript;
    [SerializeField] private Seeker seekerScript;
    [SerializeField] private TextMeshProUGUI text;

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
            gameObject.tag = seekerText;
            if (!IsOwner) return;
            text.text = seekerText;
            text.color = Color.red;
        }
        else if(playerRole == EnumPlayerState.hider)
        {
            hiderScript.enabled = true;
            gameObject.tag = hiderText;
            if (!IsOwner) return;
            text.text = hiderText;
            text.color = Color.green;
        }
    }
}
