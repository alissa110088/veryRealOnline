using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerRole : MonoBehaviour
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
            text.text = seekerText;
            text.color = Color.red;
        }
        else if(playerRole == EnumPlayerState.hider)
        {
            hiderScript.enabled = true;
            gameObject.tag = hiderText;
            text.text = hiderText;
            text.color = Color.green;
        }
    }
}
