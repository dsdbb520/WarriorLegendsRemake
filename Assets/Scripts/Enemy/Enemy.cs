using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Rigidbody2D rb;
    [HideInInspector] public Animator animator;
    [HideInInspector] public PhysicsCheck physicsCheck;
    public Character character;

    [Header("移动设置")]
    public float normalSpeed;
    public float chaseSpeed;
    [HideInInspector] public float currectSpeed;
    public float hurtForce;
    public Vector3 faceDir;

    [Header("状态")]
    public bool isHurt;
    public bool isDead;

    [Header("计时器")]
    public float waitTime;
    public float waitTimeCounter;
    public float lostTime;
    public float lostTimeCounter;
    public bool wait;
    private bool hasTurned = false;

    [Header("检测")]
    public Vector2 centerOffset;
    public Vector2 checkSize;
    public float checkDistance;
    public LayerMask attackLayer;
    public LayerMask wallLayer;

    [Header("敌人标识")]
    public string enemyID;

    protected BaseState chaseState;
    protected BaseState patrolState;
    private BaseState currentState;

    private MaterialPropertyBlock propBlock;
    private int dissolveID;
    private SpriteRenderer spriteRenderer;

    private void OnEnable()
    {
        currentState = patrolState;
        currentState.OnEnter(this);
    }

    protected virtual void Awake()
    {
        wait = false;
        waitTimeCounter = waitTime;
        lostTimeCounter = lostTime;  // BUG FIX: was never initialized, causing instant chase-exit on first detection
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        character = GetComponent<Character>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        propBlock = new MaterialPropertyBlock();
        dissolveID = Shader.PropertyToID("_DissolveAmount");
        currectSpeed = normalSpeed;
    }

    private void Update()
    {
        faceDir = new Vector3(-transform.localScale.x, 0, 0);
        currentState.LogicUpdate();
        TimeCounter();
    }

    private void FixedUpdate()
    {
        if (!isHurt && !isDead)
        {
            if (!wait) Move();
            else rb.velocity = new Vector2(0, rb.velocity.y);
        }
        currentState.PhysicsUpdate();
    }

    private void OnDisable()
    {
        currentState.OnExit();
    }

    public virtual void Move()
    {
        rb.velocity = new Vector2(currectSpeed * faceDir.x * Time.deltaTime, rb.velocity.y);
    }

    public void Turn()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    public void TimeCounter()
    {
        waitTimeCounter -= Time.deltaTime;

        if (!FoundPlayer() && lostTimeCounter >= 0)
            lostTimeCounter -= Time.deltaTime;

        // Turn on wall/edge
        if ((physicsCheck.touchedLeftWall || physicsCheck.touchedRightWall || !physicsCheck.isGround) && !hasTurned)
        {
            Turn();
            waitTime = Random.Range(2f, 4f);
            waitTimeCounter = waitTime;
            hasTurned = true;
            wait = false;
        }

        if (waitTimeCounter <= 0)
        {
            if (currentState == chaseState) return;
            if (wait)
            {
                Turn();
                waitTime = Random.Range(2f, 4f);
            }
            else
                waitTime = Random.Range(1f, 2f);
            waitTimeCounter = waitTime;
            wait = !wait;
            hasTurned = false;
        }
    }

    public bool FoundPlayer()
    {
        RaycastHit2D playerHit = Physics2D.BoxCast(
            transform.position + (Vector3)centerOffset,
            checkSize, 0, faceDir, checkDistance, attackLayer);

        if (!playerHit) return false;

        // Check line-of-sight: no wall between enemy and player
        RaycastHit2D wallHit = Physics2D.Raycast(
            transform.position + (Vector3)centerOffset,
            faceDir,
            Vector2.Distance(transform.position, playerHit.transform.position),
            wallLayer);

        return !wallHit;
    }

    public void SwitchState(NPCState state)
    {
        var newState = state switch
        {
            NPCState.Patrol => patrolState,
            NPCState.Chase  => chaseState,
            _               => null
        };
        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter(this);
    }

    #region 事件执行方法

    public void GetInjured(Transform attacker)
    {
        animator.SetTrigger("Hurt");
        isHurt = true;
        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(transform.position.x - attacker.position.x, 0).normalized;
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
    }

    public void EnemyDead()
    {
        if (isDead) return;

        gameObject.layer = 2;
        isDead = true;
        animator.SetBool("isDead", true);
        TaskManager.Instance?.UpdateTaskProgress(enemyID, 1);
        StartCoroutine(DissolveAndDestroy());
    }

    private IEnumerator DissolveAndDestroy()
    {
        yield return new WaitForSeconds(0.5f);

        float counter = 0f;
        const float duration = 1f;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float val = Mathf.Lerp(0f, 1.1f, counter / duration);
            spriteRenderer.GetPropertyBlock(propBlock);
            propBlock.SetFloat(dissolveID, val);
            spriteRenderer.SetPropertyBlock(propBlock);
            yield return null;
        }

        Destroy(gameObject);
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + (Vector3)centerOffset, checkSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + (Vector3)centerOffset + new Vector3(-checkDistance, 0, 0), checkSize);
    }
}
