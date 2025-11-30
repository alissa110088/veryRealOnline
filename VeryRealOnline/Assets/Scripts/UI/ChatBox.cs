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
    private int numMessageBeforeUp = 6;
    private float upIndex = 13f;
    private InputSystem_Actions inputActions;

    private void Start()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Player.Enable();
        inputActions.Player.chat.started += FocusOnChatBox;
    }

    private void OnDestroy()
    {
        inputActions.Player.chat.started -= FocusOnChatBox;
    }

    private void FocusOnChatBox(InputAction.CallbackContext ctx)
    {
        inputField.Select();
        inputField.ActivateInputField();
        inputField.OnPointerClick(null);
        ActionManager.DeactivateMovement.Invoke();
    }

    public void OnWritten()
    {
        DisplayTextRPC(inputField.text);
        ActionManager.ActivateMovement.Invoke();
        inputField.text = "";
    }

    [Rpc(SendTo.Everyone)]
    private void DisplayTextRPC(string pText)
    {
        textChat.text += "\n" + pText;
        index += 1;
        if(index > numMessageBeforeUp)
        {
            textChat.transform.position += new Vector3(0, upIndex,0);
        }
    }
}
