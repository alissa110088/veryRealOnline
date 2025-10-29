using Unity.Netcode;
using UnityEngine;

public class Spectate : NetworkBehaviour
{
    private int currentPosition = -1;
    private bool dead;

    [SerializeField] private PlayerNetwork playerNetwork;
    [SerializeField] private GameObject canvasGameOver;

    public void RemoveMoveOption(ulong pTarget)
    {
        transform.SetParent(null);


        if(dead)
        {
            CheckIfShouldChangeTarget(pTarget);
        }
        if (IsServer)
        {
            GameManager.instance.playersDead.Add(playerNetwork);
            GameManager.instance.playersAlive.Remove(playerNetwork);

            Destroy(playerNetwork.gameObject);
        }

        if (IsOwner && !dead)
        {
            MoveCamera(1);
            dead = true;
            canvasGameOver.SetActive(true);
        }
    }
    private void CheckIfShouldChangeTarget(ulong pTarget)
    {

        NetworkObject lNetworkObject = transform.parent.GetComponent<NetworkObject>();
        Debug.Log("here");

        if (lNetworkObject.NetworkObjectId == pTarget)
        {
            MoveCamera(1);
            Debug.Log("shouldChange");
        }
    }

    private void MoveCamera(int i)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (transform.parent != null)
            transform.SetParent(null);

        GameManager lGameManager = GameManager.instance;
        int lMax = lGameManager.playersAlive.Count;
        currentPosition += i;

        Debug.Log(currentPosition + " " + lMax);
        if (currentPosition < lMax && currentPosition >= 0)
        {
            if (lGameManager.playersAlive[currentPosition] != playerNetwork)
            {
                transform.SetParent(lGameManager.playersAlive[currentPosition].gameObject.transform);
                transform.localPosition = lGameManager.playersAlive[currentPosition].camPos;
                transform.rotation = lGameManager.playersAlive[currentPosition].gameObject.transform.rotation;
            }
            else
            {
                MoveCamera(1);
            }
        }
        else if(currentPosition >= lMax)
        {
            Debug.Log("heree");
            currentPosition = -1;
            MoveCamera(1);
        }
        else
        {
            Debug.Log("here111");
            currentPosition = lMax;
            MoveCamera(-1);
        }
    }

    public void OnNext()
    {
        MoveCamera(1);
    }

    public void OnBefore()
    {
        MoveCamera(-1);
    }

}
