using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class ConsoleUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] int maxLineCount = 10;

    int lineCount = 0;
    string myLog;

    private void OnEnable()
    {
        Application.logMessageReceived += Log;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    public void Log(string logString,  string StackTrace, LogType type)
    {
        switch (type)
        {
            case LogType.Assert:
                logString = "<color=white>" + logString + "</color>";
                break;
            case LogType.Log:
                logString = "<color=white>" + logString + "</color>";
                break;
            case LogType.Warning:
                logString = "<color=yellow>" + logString + "</color>";
                break;
            case LogType.Exception:
                logString = "<color=red>" + logString + "</color>";
                break;
            case LogType.Error:
                logString = "<color=red>" + logString + "</color>";
                break;
        }

        myLog = myLog + "\n" + logString;
        lineCount++;

        if(lineCount > maxLineCount)
        {
            lineCount++;
            myLog = DeleteLines(myLog, 1);
        }

        text.text = myLog;
    }

    private string DeleteLines(string message, int lineToRemove)
    {
       return message.Split(Environment.NewLine.ToCharArray(), lineToRemove + 1).Skip(lineToRemove).FirstOrDefault();
    }
}
