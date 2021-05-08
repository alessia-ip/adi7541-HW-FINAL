using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Log", menuName = "ScriptableObjects/Logs", order = 1)]
public class LogScriptableObjects : ScriptableObject
{
    public int logNumber;
    public string logTitle;
    public string logText;

}
