using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class ChatBox : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI textChat;
    [SerializeField] private TMP_InputField inputField;

    private int index = 0;
    private InputSystem_Actions inputActions;

    private void Start()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Player.Enable();
        inputActions.Player.chat.performed += FocusOnChatBox;
    }

    private void FocusOnChatBox(InputAction.CallbackContext ctx)
    {
        EventSystem.current.SetSelectedGameObject(inputField.gameObject, null);
        inputField.OnPointerClick(null);
        ActionManager.DeactivateMovement.Invoke();
    }

    public void OnWritten()
    {
        Debug.Log("called");
        DisplayTextRPC(inputField.text);
        ActionManager.ActivateMovement.Invoke();
        inputField.text = "";
    }

    [Rpc(SendTo.Everyone)]
    private void DisplayTextRPC(string pText)
    {
        textChat.text += "\n" + pText;
        index += 1;
        if(index > 6)
        {
            textChat.transform.position += new Vector3(0, 10,0);
        }
    }
}
