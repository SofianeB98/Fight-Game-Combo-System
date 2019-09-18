using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class AvatarController : MonoBehaviour
{
	#region Serialize Fields
	[SerializeField] private Animator anim;
	
    [Header("Movement")] 
    [SerializeField] private CharacterController characterController;
    [SerializeField, Range(0.1f, 15.0f)] private float speed = 5.0f;
    [SerializeField, Range(-1,1)] private int currentForward = 1;
    [SerializeField] private Vector3 direction = Vector3.zero;
    [SerializeField] private Vector3 orientation = Vector3.right;
    private Vector3 gravity = Vector3.zero;
    private Vector3 finalDirection = Vector3.zero;
    
    [Header("Gravity")]
    [SerializeField] private float gravityForce = 9.81f;
    [SerializeField] private float gravityModifier = 1;
    [SerializeField] private float gravityMaxSpeed = 50;

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayerMask;
    private Vector3 groundPosition;
    private bool groundDetected;
    [SerializeField] private bool grounded;
    private RaycastHit lastGroundDetectedInfos;
    [SerializeField] private float sphereGroundDetectionRadius = 0.4f;
    [SerializeField] private float groundTolerance = 0.05f;
    [SerializeField] private float characterControllerRadiusCompensator = 0.1f;

    [Header("Jump Section")] 
    [SerializeField, Range(0.3f, 0.8f)] private float jumpZoneY = 0.4f;
    [SerializeField] private Utils.InputData upLeft;
    [SerializeField] private Utils.InputData upRight;
    [SerializeField] private float jumpHeight = 8;
    [SerializeField] private float jumpTimeToReachMax = 0.5f;
    [SerializeField] private AnimationCurve jumpBehaviour;
    private float jumpTimer;
    private Vector3 jumpDir = Vector3.right;
    public bool jumping { get; private set; }
    
    
    [Header("Crounching")]
    [SerializeField, Range(-0.8f, -0.29f)] private float crounchingZone = -0.5f;
    [SerializeField] private bool isCrounching = false;

    [Header("Fighting")] 
    [SerializeField] private bool isPunching = false;
    
    [Header("Hadoken")] 
    [SerializeField] private Hadouken hadoukenPrefab;
    [SerializeField] private Transform hadoukenParent;
    private Hadouken currentHadouken = null;
    
    public CinemachineImpulseSource impulse;
    
    #endregion
    
    #region ReadOnly Fields

    public bool IsCrounching
    {
	    get { return isCrounching; }
    }
    public bool Grounded
    {
	    get { return grounded; }
    }

    #endregion
    
    #region Initialization
    private void Awake()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        if (anim == null)
	        anim = GetComponentInChildren<Animator>();
        
        anim.SetFloat("speedMovement", speed);
    }
    
	#endregion

	#region Loop

	void Update()
	{
		this.DetectGround();
		this.UpdateGravity();
        
		if (!isCrounching && grounded && !isPunching)
		{
			this.finalDirection = this.direction + this.gravity;
			characterController.Move(finalDirection * Time.deltaTime);
			anim.SetFloat("xInput", this.finalDirection.normalized.x);
			
		}
		else if (!grounded)
		{
			this.finalDirection = this.gravity + this.jumpDir;
			characterController.Move(finalDirection * Time.deltaTime);
		}
		else
		{
			characterController.Move(Vector3.zero);
		}
        
		this.GroundPositionCorrection();
	}

	#endregion
	
    #region Controller Functions
    public void UpdateMovement(Vector3 dir)
    {
        dir = dir.normalized;

        isCrounching = dir.y <= crounchingZone;
        anim.SetBool("isCrounching", isCrounching);
        
        dir *= speed;
        direction = new Vector3(dir.x,0,0);

        if (dir.y > jumpZoneY)
        {
	        UpdateJump(dir.normalized);
        }
	        

    }
    
	private void DetectGround() 
	{
		this.groundDetected = Physics.SphereCast(new Vector3(this.transform.position.x, this.transform.position.y - this.characterController.height/2 + this.sphereGroundDetectionRadius - this.characterControllerRadiusCompensator, this.transform.position.z),
			this.sphereGroundDetectionRadius,
			Vector3.down,
			out this.lastGroundDetectedInfos,
			4,
		    groundLayerMask);
	}

	private void UpdateGravity() 
	{
		if (!this.grounded && !this.jumping) 
		{
			this.gravity = new Vector3(0,this.gravity.y - this.gravityForce * this.gravityModifier * Time.deltaTime,0);

			if (Mathf.Abs(this.gravity.y) > this.gravityMaxSpeed) 
			{
				this.gravity = new Vector3(0,-this.gravityMaxSpeed,0);
			}
		}
		else if(this.jumping) 
		{
			this.jumpTimer += Time.deltaTime / this.jumpTimeToReachMax;
			if (this.jumpTimer >= 1.0f) 
			{
				this.jumping = false;
            }
			else 
			{
				var velocity = this.jumpBehaviour.Evaluate(this.jumpTimer + Time.deltaTime / this.jumpTimeToReachMax) - this.jumpBehaviour.Evaluate(this.jumpTimer);
				this.gravity = new Vector3(0,velocity * this.jumpHeight / Time.deltaTime,0);
			}
		}
		else 
		{
			this.gravity = new Vector3(0,0,0);
        }
	}

	private void GroundPositionCorrection() 
	{
		if (this.transform.position.y - this.characterControllerRadiusCompensator - this.characterController.height/2 < this.lastGroundDetectedInfos.point.y - this.groundTolerance) 
		{
			this.characterController.Move(new Vector3(0, Vector3.Distance(this.lastGroundDetectedInfos.point,new Vector3(this.transform.position.x,this.transform.position.y - this.characterControllerRadiusCompensator - this.characterController.height/2, this.transform.position.z)), 0));
			this.grounded = true;
		}
		else if(Mathf.Abs(this.transform.position.y - this.characterControllerRadiusCompensator - this.characterController.height/2 - this.lastGroundDetectedInfos.point.y) > this.groundTolerance)
		{
			this.grounded = false;
		}
		else {
			this.grounded = true;
			anim.SetBool("isJumping", false);
		}
	}

	public void UpdateJump(Vector3 dir) 
	{
		if (this.grounded)
		{
            this.jumping = true;
            this.jumpTimer = 0.0f;
            
            if (upLeft.xInputZone.x < dir.x && upLeft.xInputZone.y > dir.x && 
                upLeft.yInputZone.x < dir.y && upLeft.yInputZone.y > dir.y)
            {
	            jumpDir = Vector3.right * -1 * speed;
            }
            else if (upRight.xInputZone.x < dir.x && upRight.xInputZone.y > dir.x && 
                     upRight.yInputZone.x < dir.y && upRight.yInputZone.y > dir.y)
            {
	            jumpDir = Vector3.right * 1 * speed;
            }
            else
            {
	            jumpDir = Vector3.zero;
            }
            
            anim.SetBool("isJumping", jumping);
		}
	}
    #endregion

    #region Fighting Functions

    public void Punching()
    {
	    anim.SetTrigger("Punching");
	    isPunching = true;
    }

    public void ResetPunchingBool()
    {
	    Debug.Log("Reset Punching");
	    isPunching = false;
	    Debug.Log("puching = " + isPunching);
    }

    public void Hadoken()
    {
	    isPunching = true;
	    UpdateMovement(Vector3.zero);
	    anim.ResetTrigger("Punching");
	    currentHadouken = Instantiate(hadoukenPrefab, hadoukenParent);
	    currentHadouken.Initialise(transform.forward);
    }

    public void FreeHadoken()
    {
	    currentHadouken.RemoveParent();
	    currentHadouken = null;
	    impulse.GenerateImpulse();
    }
    
    #endregion

    #region Debug Functions

    private void OnDrawGizmos() 
    {
		
    }

    private void OnDrawGizmosSelected() 
    {
	    Gizmos.color = Color.yellow;
	    Gizmos.DrawWireSphere(new Vector3(this.transform.position.x, this.transform.position.y - this.characterController.height/2 + this.sphereGroundDetectionRadius - characterControllerRadiusCompensator, this.transform.position.z),
		    this.sphereGroundDetectionRadius);
	    Gizmos.color = Color.red;
	    Gizmos.DrawWireSphere(this.lastGroundDetectedInfos.point, 0.05f);
	    Gizmos.color = Color.green;
	    Gizmos.DrawRay(this.transform.position,this.direction);
	    Gizmos.color = Color.blue;
	    Gizmos.DrawRay(new Vector3(this.transform.position.x,this.transform.position.y + 0.4f,this.transform.position.z),this.orientation * 4);
	    Gizmos.color = Color.magenta;
	    Gizmos.DrawRay(this.transform.position,this.gravity);
    }

    #endregion

}
