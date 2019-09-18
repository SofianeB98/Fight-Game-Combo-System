using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class AvatarFighterManager : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private Transform avatar;
    
    [Header("Combo Manager")]
    [SerializeField] private List<Utils.ComboInput> currentCombo = new List<Utils.ComboInput>();
    [SerializeField] private int maxComboListSize = 20;
    [SerializeField] private List<Utils.ComboData> avatarCombos = new List<Utils.ComboData>();
    [SerializeField] private List<Utils.InputData> inputs = new List<Utils.InputData>();
    [SerializeField] private Utils.ComboInput lastAxis = Utils.ComboInput.Carre;
    
    [Header("Combo Timer")] 
    [SerializeField] private float maxDelayBetweenTwoInputs = 0.25f;
    [SerializeField] private float currentDelay = 0.0f;

    private void Awake()
    {
        if (anim == null)
            anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        //Check si on est dans le temps du combo
        if (Time.time > currentDelay && currentCombo.Count > 0)
        {
            currentCombo.Clear();
        }
    }

    #region Combo Functions

    /// <summary>
    /// button est le button presser dans le cas d'un flags 1'
    /// Flags = 1 un button, 0 une direction
    /// dir est le vecteur directeur dans le cas d'un flag 0'
    /// </summary>
    /// <param name="button"></param>
    /// <param name="flag"></param>
    /// <param name="dir"></param>
    public void AddCombo(GamepadButton button, int flag, Vector2 dir)
    {
        if (flag == 0 && dir == Vector2.zero)
        {
            lastAxis = Utils.ComboInput.Carre;
        }
        
        foreach (Utils.InputData inputData in inputs)
        {
            if (inputData.flags == flag)
            {
                switch (flag)
                {
                    case 0:
                        if (inputData.xInputZone.x < dir.x && inputData.xInputZone.y > dir.x && 
                            inputData.yInputZone.x < dir.y && inputData.yInputZone.y > dir.y)
                        {
                            if (inputData.input != lastAxis)
                            {
                                currentCombo.Add(inputData.input);
                                lastAxis = inputData.input;
                                currentDelay = Time.time + maxDelayBetweenTwoInputs;
                            }
                        }
                        break;
                    
                    case 1:
                        if (inputData.button == button)
                        {
                            currentCombo.Add(inputData.input);
                            currentDelay = Time.time + maxDelayBetweenTwoInputs;
                        }
                        break;
                    
                    default:
                        Debug.LogError("Mauvais flag pour " + button + " " + dir);
                        break;
                }
            }
        }
        
        ComboManager(currentCombo);
    }

    private void ComboManager(List<Utils.ComboInput> currentCombo)
    {
        foreach (Utils.ComboData combos in avatarCombos)
        {
            if (CheckCombo(currentCombo, combos.inputsCombo))
            {
                if (combos.yRotation >= avatar.transform.eulerAngles.y - 0.1f && combos.yRotation <= avatar.transform.eulerAngles.y + 0.1f)
                {
                    anim.SetTrigger(combos.triggerName);
                    currentCombo.Clear();
                }
            }
        }
        
        if (currentCombo.Count >= maxComboListSize)
        {
            currentCombo.Clear();
        }
    }

    private bool CheckCombo(List<Utils.ComboInput> currentCombo, List<Utils.ComboInput> checkCombo)
    {
        if (currentCombo.Count != checkCombo.Count)
            return false;

        for (int i = 0; i < currentCombo.Count; i++)
        {
            if (currentCombo[i] != checkCombo[i])
            {
                Debug.Log("le combo est pas bon a l'input + '"  + i);
                return false;
            }
        }

        return true;
    }

    #endregion
    
    
    
    
}
