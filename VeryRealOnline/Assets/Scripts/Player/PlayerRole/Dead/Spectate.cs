using Unity.Netcode;
using UnityEngine;

public class Spectate : NetworkBehaviour
{
    private int currentPosition = -1;
    private bool dead;

    [SerializeField] private PlayerNetwork playerNetwork;
    [SerializeField] private GameObject canvasGameOver;

    public void RemoveMoveOption(ulong pTarget, GameObject pChange)
    {
        transform.SetParent(null);

        if (IsOwner && playerNetwork.NetworkObject.NetworkObjectId == pTarget && !dead)
        {
            InitDead(pChange);
        }
        if (IsServer)
        {
            DestroyObject();
        }
    }


    private void DestroyObject()
    {
        GameManager.instance.playersDead.Add(playerNetwork);
        GameManager.instance.playersAlive.Remove(playerNetwork);
        Destroy(playerNetwork.gameObject);

    }


    private void InitDead(GameObject pToSpectate)
    {
        if (IsOwner && !dead)
        {
            transform.SetParent(pToSpectate.transform);
            transform.localPosition = pToSpectate.GetComponent<PlayerNetwork>().camPos;
            transform.rotation = pToSpectate.transform.rotation;

            dead = true;
            canvasGameOver.SetActive(true);
        }
    }
}
