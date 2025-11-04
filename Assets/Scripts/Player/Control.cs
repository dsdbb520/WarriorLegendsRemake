using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class Control : MonoBehaviour
{
    private Animator animator;
    private CapsuleCollider2D coll;
    public PhysicsMaterial2D wall;
    public PhysicsMaterial2D ground;
    public PlayerControl inputActions;
    public PlayerAnimation PlayerAnimation;
    public Rigidbody2D rb;
    public PhysicsCheck physicsCheck;
    private int face;
    public Vector2 inputDirection;
    [Header("参数")]
    public float speed;
    public float jumpForce;
    public float injuredForce;
    public float attackForce;
    public float coyoteTime;  //土狼时间
    [Header("状态")]
    public bool isInjured;
    public bool isDead;
    public bool isAttack;
    public bool isJump;
    [Header("交互检测")]
    public float interactRange = 1.3f;
    public Vector2 interactOffset = new Vector2(0f, 1.0f);
    public LayerMask interactLayer;
    private IInteractable currentTarget; // 当前检测到的可交互对象
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();
        PlayerAnimation = GetComponent<PlayerAnimation>();
        animator = GetComponent<Animator>();
        inputActions = new PlayerControl();
        physicsCheck=GetComponent<PhysicsCheck>();
        inputActions.Gameplay.Jump.started += Jump;
        inputActions.Gameplay.Fire.started += Fire;
        inputActions.Gameplay.Interact.started += Interact;
    }


    private void OnEnable()
    {
        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        inputDirection=inputActions.Gameplay.Move.ReadValue<Vector2>();
        CheckState();
        DetectInteractable(); // 每帧更新可交互对象
    }

    private void FixedUpdate()
    {
        if (!isInjured&&!isAttack)
        {
                Move();
        }
    }

    public void Move()
    {
        if (!PlayerActionManager.Instance.canMove) return;
        rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);
        face = (int)transform.localScale.x;
        if (inputDirection.x < 0) face = -1;
        if (inputDirection.x > 0) face = 1;
        //人物翻转
        transform.localScale = new Vector3(face, 1, 1);
    }
    private void Jump(InputAction.CallbackContext context)
    {
        if ( ( physicsCheck.isGround || ( physicsCheck.hangTime < coyoteTime ) ) && !isJump && PlayerActionManager.Instance.canJump) 
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            isJump = true;
            StartCoroutine(JumpAgain());
        }
    }
    private IEnumerator JumpAgain()
    {
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => physicsCheck.isGround);
        isJump = false;
    }
    private void Fire(InputAction.CallbackContext context)
    {
        if (!isInjured && PlayerActionManager.Instance.canAttack)
        {
            isAttack = true;
            PlayerAnimation.Attack();
            Vector2 atkDir = new Vector2(transform.localScale.x, 0);
            rb.AddForce(atkDir * attackForce, ForceMode2D.Impulse);
        }
    }

    private void Interact(InputAction.CallbackContext context)
    {
        if (!PlayerActionManager.Instance.canInteract) return;
            Debug.Log("按下了F键");
        if (currentTarget != null)
        {
            Debug.Log("调用了接口");
            currentTarget.Interact(); // 调用接口方法
        }
    }
    private void DetectInteractable()    //寻找周围的可交互对象
    {
        
        Vector2 detectPos = (Vector2)transform.position + new Vector2(interactOffset.x * transform.localScale.x, interactOffset.y);
        Collider2D hit = Physics2D.OverlapCircle(detectPos, interactRange, interactLayer);
        currentTarget = hit != null ? hit.GetComponent<IInteractable>() : null;
    }

    //这两个函数是动画事件调用的
    public void PlayAttack1Sound()
    {
        AudioManager.Instance.PlayCharacterSound("attack1");
    }
    public void PlayAttack2Sound()
    {
        AudioManager.Instance.PlayCharacterSound("attack2");
    }


    public void GetInjured(Transform attacker)
    {
        isInjured = true;
        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(transform.position.x - attacker.position.x, 0).normalized;
        rb.AddForce(dir * injuredForce, ForceMode2D.Impulse);
    }

    public void Dead()
    {
        isDead = true;
        inputActions.Gameplay.Disable();
    }

    public void CheckState()
    {
        coll.sharedMaterial = physicsCheck.isGround ? ground : wall;
    }
    private void OnDrawGizmosSelected()   //交互区域的可视化
    {
        Gizmos.color = Color.yellow;
        Vector2 detectPos = (Vector2)transform.position + new Vector2(interactOffset.x * transform.localScale.x, interactOffset.y);
        Gizmos.DrawWireSphere(detectPos, interactRange);
    }
}
