using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChatBox : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI textChat;
    [SerializeField] private TMP_InputField inputField;

    private int index = 0;
    private int numMessageBeforeUp = 6;

    private InputSystem_Actions inputActions;
    private List<string> messages = new List<string>();

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
        messages.Add(pText);
        index += 1;
        if(index > numMessageBeforeUp)
        {
            messages.RemoveAt(0);
        }
        UpdateText();
    }

    private void UpdateText()
    {
        textChat.text = "";
        for (int i = 0; i < messages.Count; i++)
        {
            textChat.text += "\n" + messages[i];
        }
    }
}
