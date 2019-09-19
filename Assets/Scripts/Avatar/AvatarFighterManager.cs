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
    [SerializeField, 
     Tooltip("Les combos doivent être rangé du plus long au plus court")] private List<ComboData> avatarCombos = new List<ComboData>();
    [SerializeField] private List<InputData> inputs = new List<InputData>();
    [SerializeField] private Utils.ComboInput lastAxis = Utils.ComboInput.Carre;
    [SerializeField, Tooltip("le count minimal pour reset le current combo")] private int minComboLenghtToResetCombo = 4;
    private int lastCount = 0;
    
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
            lastCount = 0;
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
            return;
        }
        
        foreach (InputData inputData in inputs)
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

        if (currentCombo.Count > lastCount)
        {
            ComboManager(currentCombo);
            lastCount = currentCombo.Count;
        }
        
    }

    private void ComboManager(List<Utils.ComboInput> currentCombo)
    {
        foreach (ComboData combos in avatarCombos)
        {
            if (CheckCombo(currentCombo, combos.inputsCombo))
            {
                if (combos.yRotation >= avatar.transform.eulerAngles.y - 0.1f && combos.yRotation <= avatar.transform.eulerAngles.y + 0.1f)
                {
                    anim.SetTrigger(combos.triggerName);
                    break;
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
        if (checkCombo.Count > currentCombo.Count)
            return false;
        
        if (currentCombo[currentCombo.Count - 1] != checkCombo[checkCombo.Count - 1])
            return false;
        
        
        int correctInput = 0;
        for (int i = 0; i < currentCombo.Count; i++)
        {
            if (currentCombo[i] == checkCombo[correctInput])
                correctInput++;
            else
                correctInput = 0;

            if (correctInput == checkCombo.Count)
                break;
        }

        if (correctInput == checkCombo.Count)
        {
            if (correctInput >= minComboLenghtToResetCombo)
            {
                currentCombo.Clear();
                lastCount = 0;
            }
                
            
            return true;
        }
        else
            return false;
        
        
    }

    #endregion
    
    
    
    
}
