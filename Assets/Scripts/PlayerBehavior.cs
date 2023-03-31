using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerBehavior : SerializedMonoBehaviour
{
    [Title("Properties")]

    [SerializeField] private float MoveSpeed;
    [SerializeField] private float JumpPower;
    [SerializeField] private float StepInterval = 0.5f;

    [Title("Raycasts")]

    [SerializeField] Transform RCO_FootR;
    [SerializeField] Transform RCO_FootL;
    [SerializeField] float GroundedLength = 0.2f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] CircleCollider2D rootCaptureZone;
    [SerializeField] LayerMask rootLayer;
    
    [Title("Controls")]

    public KeyCode Key_Right;
    public KeyCode Key_Left;
    public KeyCode Key_Up;
    public KeyCode Key_Down;

    public KeyCode Key_Jump;
    public KeyCode Key_Plant;
    public KeyCode Key_ShiftControl;
    public KeyCode Key_RootUndo;

    public KeyCode Key_Reset;

    [Title("Prefabs")]
    public GameObject StepParticle;

    public enum ControlMode
    {
        Normal,
        Root
    }

     public ControlMode currentControlmode;
    [HideInInspector] public Rigidbody2D rbody;
    [HideInInspector] public UnityAction OnShiftToRoot;
    [HideInInspector] public UnityAction OnShiftToNormal;

    public Root CurrentControllingRoot;

    private Animator anim;
    private SimpleSoundModule soundModule;

    //public GameObject highlight;


    private void Awake()
    {
        rbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        soundModule = GetComponent<SimpleSoundModule>();
    }

    float moveLerpT = 0.25f;
    float stepT = 0f;

    private void FixedUpdate()
    {
        anim.SetBool("Grounded", IsGrounded());
        anim.SetFloat("VertSpeed", rbody.velocity.y);
        stepT -= Time.fixedDeltaTime;

        TryCaptureSeed();

        if (currentControlmode == ControlMode.Normal)
        {
            if (Input.GetKey(Key_Right))
            {
                rbody.velocity = new Vector2(Mathf.Lerp(rbody.velocity.x, MoveSpeed, moveLerpT), rbody.velocity.y);
                transform.right = Vector2.right;
                anim.SetBool("HorInput", true);
                TryStep();
            }
            else if(Input.GetKey(Key_Left))
            {
                rbody.velocity = new Vector2(Mathf.Lerp(rbody.velocity.x, -MoveSpeed, moveLerpT), rbody.velocity.y);
                transform.right = Vector2.left;
                anim.SetBool("HorInput", true);
                TryStep();
            }
        }

        if(!Input.GetKey(Key_Right)&&!Input.GetKey(Key_Left))
        {
            rbody.velocity = new Vector2(Vector2.Lerp(rbody.velocity, Vector2.zero, moveLerpT).x, rbody.velocity.y);
            anim.SetBool("HorInput", false);
        }
        
    }

    private void Update()
    {
        if(Input.GetKeyDown(Key_Reset))
        {
            SceneLoader.Instance.StartLoadingScene(SceneManager.GetActiveScene().name);
            soundModule.Play("Reset");
        }

        if (currentControlmode == ControlMode.Normal)
        {
            if (Input.GetKeyDown(Key_Jump) && IsGrounded())
            {
                rbody.velocity = new Vector2(rbody.velocity.x, JumpPower);
                soundModule.Play("Jump");
            }
            if (Input.GetKeyDown(Key_ShiftControl) && CurrentControllingRoot)
            {
                if (OnShiftToRoot != null)
                    OnShiftToRoot.Invoke();
                currentControlmode = ControlMode.Root;
                CurrentControllingRoot.OpenUI();
             
            }
        }
        else if (currentControlmode == ControlMode.Root)
        {
            if (CurrentControllingRoot)
            {
                CurrentControllingRoot.childCam.gameObject.SetActive(true);

                if (Input.GetKeyDown(Key_Up))
                {
                    if (CurrentControllingRoot.LiftMode)
                    {

                        if (CurrentControllingRoot.PressUp()) soundModule.Play("StemGrow");
                    }
                    if (CurrentControllingRoot.PressUp()) soundModule.Play("StemGrow");
                }
                if (Input.GetKeyDown(Key_Right))
                {
                    if(CurrentControllingRoot.PressRight()) soundModule.Play("StemGrow");
                }
                if (Input.GetKeyDown(Key_Left))
                {
                    if(CurrentControllingRoot.PressLeft()) soundModule.Play("StemGrow");
                }
                if (Input.GetKeyDown(Key_Down))
                {
  
                       if( CurrentControllingRoot.PressDown()) soundModule.Play("StemGrow");


                }
                if (Input.GetKeyDown(Key_RootUndo))
                {
                    CurrentControllingRoot.PressUndo();
                }


                if (Input.GetKeyDown(Key_ShiftControl))
                {
                    CurrentControllingRoot.childCam.gameObject.SetActive(false);
                }

                if (CurrentControllingRoot.LiftMode)
                {
                    CurrentControllingRoot.RefreshPlayerPositionOnLiftMode(this.transform);
                }
            }

                if (Input.GetKeyDown(Key_ShiftControl))
                {
                    if (OnShiftToNormal != null)
                        OnShiftToNormal.Invoke();

                    currentControlmode = ControlMode.Normal;

                CurrentControllingRoot.CloseUI();
            }
            }
        
    }

    public bool IsGrounded()
    {
        bool footR = Physics2D.Raycast(RCO_FootR.position, Vector2.down, GroundedLength, groundLayer);
        bool footL = Physics2D.Raycast(RCO_FootL.position, Vector2.down, GroundedLength, groundLayer);

        Debug.DrawRay(RCO_FootR.position, Vector3.down);
        Debug.DrawRay(RCO_FootL.position, Vector3.down);

        return footR || footL;
    }

    private void TryStep()
    {
        if (stepT < 0f)
        {
            if (!IsGrounded()) return;

            stepT = StepInterval;
            Instantiate(StepParticle, RCO_FootL.position,Quaternion.Euler(0,0, UnityEngine.Random.Range(0.0f, 360f)));

            var hit = Physics2D.Raycast(RCO_FootR.position, Vector2.down, GroundedLength, groundLayer);
            if (hit)
            {
                if (hit.collider.gameObject.tag == "Grass")
                {
                    soundModule.Play("Walk_Grass");
                }
                else if (hit.collider.gameObject.tag == "Wood")
                {
                    soundModule.Play("Walk_Wood");
                }
            }
        }
    }

    private void TryCaptureSeed()
    {
        RaycastHit2D rhit = Physics2D.CircleCast(transform.position, rootCaptureZone.radius, Vector2.down, rootCaptureZone.radius, rootLayer);
        if(rhit)
        {
            rhit.collider.TryGetComponent(out CurrentControllingRoot);
        }
        else if (!rhit && currentControlmode == ControlMode.Normal)
        {
            CurrentControllingRoot = null;
        }
    }
    
}
