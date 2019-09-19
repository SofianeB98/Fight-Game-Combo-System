using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

[CreateAssetMenu(fileName = "NewInputData", menuName = "Input Data/New Input Data", order = 0)]
public class InputData : ScriptableObject
{
    [Header("Input")] 
    public Utils.ComboInput input;
        
    [Header("Flags")]
    [Range(0,1), Tooltip("0 = Axis, 1 = button")] public int flags = 0;

    [Header("Button")]
    public GamepadButton button;

    [Header("Axis")] 
    public Vector2 xInputZone = new Vector2(0,0);
    public Vector2 yInputZone = new Vector2(0,0);
}
