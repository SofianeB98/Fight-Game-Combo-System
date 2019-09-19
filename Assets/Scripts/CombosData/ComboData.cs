using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewComboData", menuName = "Combo Data/New Combo Data", order = 0)]
public class ComboData : ScriptableObject
{
    [Header("Combo")] 
    [Tooltip("La suite d'input a effectuer")] public List<Utils.ComboInput> inputsCombo; 
    [Tooltip("Le nom du trigger qui declenche l'animation")] public string triggerName = "Hadoken";

    [Header("Conditions")] 
    [Tooltip("la rotation en Y que l'avatar doit avoir")] public float yRotation = 90.0f;

}
