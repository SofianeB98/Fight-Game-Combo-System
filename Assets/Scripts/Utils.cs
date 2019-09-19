using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.LowLevel;

public class Utils
{
    public enum ComboInput {Carre, Triangle, Rond, Croix, Up, UpRight, Right, DownRight, Down, DownLeft, Left, UpLeft}

    [System.Serializable]
    public class FloatEvent : UnityEvent<float>
    {
        
    }

}
