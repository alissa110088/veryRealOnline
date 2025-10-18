using UnityEngine;

public class PlayerRole : MonoBehaviour
{
    [SerializeField] private EnumPlayerState playerRole;
    [SerializeField] private Hider hiderScript;
    [SerializeField] private Seeker seekerScript;

    private void Start()
    {
        AttributeRole();
    }
    private void AttributeRole()
    {
        if(playerRole == EnumPlayerState.seeker)
        {
            seekerScript.enabled = true;
        }
        else if(playerRole == EnumPlayerState.hider)
        {
            hiderScript.enabled = true; ;
        }
    }

}
