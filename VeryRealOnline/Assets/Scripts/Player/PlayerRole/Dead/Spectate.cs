using Unity.Netcode;
using UnityEngine;

public class Spectate : NetworkBehaviour
{
    private int currentPosition = -1;
    [SerializeField] private PlayerNetwork playerNetwork;

    public void RemoveMoveOption()
    {
        transform.SetParent(null);

        if(IsServer)
            Destroy(playerNetwork.gameObject);

        if(IsOwner)
            MoveCamera();

    }

    private void MoveCamera()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (transform.parent != null)
            transform.SetParent(null);

        GameManager lGameManager = GameManager.instance;
        int lMax = lGameManager.Players.Count;
        currentPosition++;

        if (currentPosition < lMax)
        {
            if (lGameManager.Players[currentPosition] != playerNetwork)
            {
                transform.SetParent(lGameManager.Players[currentPosition].gameObject.transform);
                transform.localPosition = lGameManager.Players[currentPosition].camPos;
                transform.rotation = lGameManager.Players[currentPosition].gameObject.transform.rotation;
            }
            else
            {
                MoveCamera();
            }
        }
        else
        {
            currentPosition = -1;
            MoveCamera();
        }
    }

}
