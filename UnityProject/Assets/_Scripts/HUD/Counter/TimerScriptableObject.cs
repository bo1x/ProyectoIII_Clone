using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "TimerData", menuName = "ScriptableObjects/Utility/TimerData", order = 1)]
public class TimerScriptableObject : ScriptableObject
{
   public TimerImageArray[] timerImageArray;
}

[System.Serializable]
public class TimerImageArray
{
    public Sprite sprite;
    public Vector2 scale;
    public string AnimationInvoke;
}
