using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerRole : NetworkBehaviour
{
    private EnumPlayerState playerRole;
    private string hiderText = "hider";
    private string seekerText = "seeker";
    private Vector3 pos;

    [SerializeField] private Hider hiderScript;
    [SerializeField] private Seeker seekerScript;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Material redTexture;

    private void OnEnable()
    {
        ActionManager.GivePlayerRole += GiveRole;
    }

    private void OnDisable()
    {
        ActionManager.GivePlayerRole -= GiveRole;
    }

    private void GiveRole(EnumPlayerState pState, GameObject pPlayer, Vector3 pPosition)
    {
        if (pPlayer != gameObject)
            return;
        playerRole = pState;
        pos = pPosition;
        AttributeRole();
    }
    private void AttributeRole()
    {
        if(playerRole == EnumPlayerState.seeker)
        {
            hiderScript.enabled = true;
            seekerScript.enabled = true;
            gameObject.tag = seekerText;
            meshRenderer.material = redTexture;

            if (!IsOwner) return;
            text.text = seekerText;
            text.color = Color.red;
            transform.position = pos;
        }
        else if(playerRole == EnumPlayerState.hider)
        {
            hiderScript.enabled = true;
            gameObject.tag = hiderText;

            if (!IsOwner) return;
            text.text = hiderText;
            text.color = Color.green;
            transform.position = pos;
        }
    }
}
