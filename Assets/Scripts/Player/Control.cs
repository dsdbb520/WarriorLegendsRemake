using System;
using System.Collections;
using System.Collections.Generic;
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

    [Header("参数（非属性系统控制）")]
    public float injuredForce;
    public float attackForce;
    public float coyoteTime;  // 土狼时间

    [Header("状态")]
    public bool isInjured;
    public bool isDead;
    public bool isAttack;
    public bool isJump;

    [Header("交互检测")]
    public float interactRange = 1.3f;
    public Vector2 interactOffset = new Vector2(0f, 1.0f);
    public LayerMask interactLayer;
    private IInteractable currentTarget;

    private Character character;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();
        PlayerAnimation = GetComponent<PlayerAnimation>();
        animator = GetComponent<Animator>();
        inputActions = new PlayerControl();
        physicsCheck = GetComponent<PhysicsCheck>();
        character = GetComponent<Character>(); // 读取角色属性系统

        if (character == null)
        {
            Debug.LogError("Control.cs 找不到 Character 组件，请确保玩家上挂载了 Character.cs！");
        }

        inputActions.Gameplay.Jump.started += Jump;
        inputActions.Gameplay.Fire.started += Fire;
        inputActions.Gameplay.Interact.started += Interact;
        inputActions.Gameplay.Save.started += Save;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }

    void Update()
    {
        inputDirection = inputActions.Gameplay.Move.ReadValue<Vector2>();
        CheckState();
        DetectInteractable();
    }

    private void FixedUpdate()
    {
        if (!isInjured && !isAttack)
        {
            Move();
        }
    }

    public void Move()
    {
        if (!PlayerActionManager.Instance.canMove || character == null) return;

        //使用属性系统的移动速度
        float moveSpeed = character.stats.moveSpeed;

        rb.velocity = new Vector2(inputDirection.x * moveSpeed * Time.deltaTime, rb.velocity.y);

        face = (int)transform.localScale.x;
        if (inputDirection.x < 0) face = -1;
        if (inputDirection.x > 0) face = 1;

        transform.localScale = new Vector3(face, 1, 1);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if ((physicsCheck.isGround || (physicsCheck.hangTime < coyoteTime)) && !isJump && PlayerActionManager.Instance.canJump)
        {
            //使用属性系统的跳跃力
            float jumpPower = character != null ? character.stats.jumpForce : 5f;
            rb.AddForce(transform.up * jumpPower, ForceMode2D.Impulse);
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
        if (currentTarget != null)
        {
            currentTarget.Interact();
        }
    }

    private void Save(InputAction.CallbackContext context)
    {
        SaveSystemJSON.SavePlayer(GetComponent<Character>());
    }

    private void DetectInteractable()
    {
        Vector2 detectPos = (Vector2)transform.position + new Vector2(interactOffset.x * transform.localScale.x, interactOffset.y);
        Collider2D hit = Physics2D.OverlapCircle(detectPos, interactRange, interactLayer);
        currentTarget = hit != null ? hit.GetComponent<IInteractable>() : null;
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector2 detectPos = (Vector2)transform.position + new Vector2(interactOffset.x * transform.localScale.x, interactOffset.y);
        Gizmos.DrawWireSphere(detectPos, interactRange);
    }
}
