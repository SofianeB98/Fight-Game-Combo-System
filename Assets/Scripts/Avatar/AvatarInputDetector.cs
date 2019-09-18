using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

public class AvatarInputDetector : MonoBehaviour
{
    [SerializeField] private AvatarController avatarController;
    [SerializeField] private AvatarFighterManager avatarFighterManager;

    [Header("Input")] 
    [SerializeField] private float xInput = 0;
    [SerializeField] private float yInput = 0;
    [SerializeField, Range(0.0f, 0.65f)] private float deadZone = 0.25f;
    [SerializeField] private GamepadButton punching;
    [SerializeField] private bool isPressed = false;
    
    private Gamepad gamePad;

    private void Awake()
    {
        if (avatarController == null)
            avatarController = GetComponent<AvatarController>();

        if (avatarFighterManager == null)
            avatarFighterManager = GetComponent<AvatarFighterManager>();
    }

    // Update is called once per frame
    void Update()
    {
        gamePad = Gamepad.current; // sert a avoir les inputs

        #region Movement

        xInput = gamePad.leftStick.x.ReadValue();
        yInput = gamePad.leftStick.y.ReadValue();
        
        if ( Mathf.Abs(xInput) > deadZone ||  Mathf.Abs(yInput) > deadZone)
        {
            Vector3 dir = gamePad.leftStick.ReadValue();
            avatarController.UpdateMovement(dir);
            avatarFighterManager.AddCombo(GamepadButton.Cross, 0, dir.normalized);
        }
        else
        {
            avatarController.UpdateMovement(Vector3.zero);
            avatarFighterManager.AddCombo(GamepadButton.Cross, 0, Vector2.zero);
        }

        #endregion

        #region Fighting

        if (!avatarController.IsCrounching && avatarController.Grounded)
        {
            if (gamePad[punching].isPressed && !isPressed)
            {
                avatarFighterManager.AddCombo(punching, 1, Vector2.zero);
                isPressed = true;
                avatarController.Punching();
            }
            else if(!gamePad[punching].isPressed)
            {
                isPressed = false;
            }
        }
        

        #endregion
        
    }
}
