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

    [CreateAssetMenu(fileName = "NewInputData", menuName = "Input Data/New Input Data", order = 0)]
    public class InputData : ScriptableObject
    {
        [Header("Input")] 
        public ComboInput input;
        
        [Header("Flags")]
        [Range(0,1), Tooltip("0 = Axis, 1 = button")] public int flags = 0;

        [Header("Button")]
        public GamepadButton button;

        [Header("Axis")] 
        public Vector2 xInputZone = new Vector2(0,0);
        public Vector2 yInputZone = new Vector2(0,0);
    }
    
    [CreateAssetMenu(fileName = "NewComboData", menuName = "Combo Data/New Combo Data", order = 0)]
    public class ComboData : ScriptableObject
    {
        [Header("Combo")] 
        [Tooltip("La suite d'input a effectuer")] public List<ComboInput> inputsCombo; 
        [Tooltip("Le nom du trigger qui declenche l'animation")] public string triggerName = "Hadoken";

        [Header("Conditions")] 
        [Tooltip("la rotation en Y que l'avatar doit avoir")] public float yRotation = 90.0f;

    }
}
