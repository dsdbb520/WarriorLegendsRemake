using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Rigidbody2D rb;
    [HideInInspector]public Animator animator;
    [HideInInspector] public PhysicsCheck physicsCheck;
    public Character character;

    [Header("基本参数")]
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

    [Header("任务识别")]
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
            if (!wait)
            {
                Move();
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y); // 停止水平移动
            }
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
        // 计时器递减
        waitTimeCounter -= Time.deltaTime;
        if (!FoundPlayer() && lostTimeCounter >= 0)
        {
            lostTimeCounter -= Time.deltaTime;
        }


        //撞墙回头
        if ((physicsCheck.touchedLeftWall || physicsCheck.touchedRightWall || !physicsCheck.isGround) && !hasTurned)
        {
            Turn();                                 // 转身
            waitTime = Random.Range(2f, 4f);       // 下一次随机移动时间
            waitTimeCounter = waitTime;
            hasTurned = true;                       // 标记已经转过
            wait = false;
        }

        //随机时间到也回头
        if (waitTimeCounter <= 0)
        {
            if (currentState == chaseState) return;      //如果当前是追击状态，直接跳过计时
            if (wait)
            {
                Turn();                              //转身
                waitTime = Random.Range(2f, 4f);   //停下后的随机等待时间
            }
            else
                waitTime = Random.Range(1f, 2f);   //移动后的随机时间
            waitTimeCounter = waitTime;            //重置计时器
            wait = !wait;                           //切换等待/移动状态
            hasTurned = false;                      //重置碰墙标志，允许下一次触发
        }
    }

    public bool FoundPlayer()
    {
        //发射Box检测是否碰到玩家
        RaycastHit2D playerHit = Physics2D.BoxCast(transform.position + (Vector3)centerOffset,
                                           checkSize, 0, faceDir, checkDistance, attackLayer);
        if (playerHit)
        {
            // 再从敌人位置发射一条直线到玩家，看中间有没有墙
            RaycastHit2D wallHit = Physics2D.Raycast(transform.position + (Vector3)centerOffset,
                                                     faceDir,
                                                     Vector2.Distance(transform.position, playerHit.transform.position),
                                                     wallLayer);

            if (!wallHit)
            {
                Debug.Log("玩家在视野中且没有被墙挡住");
                return true;
            }
            else
            {
                Debug.Log("玩家被墙挡住");
                return false;
            }
        }
        return false;
    }

    public void SwitchState(NPCState state)    //切换状态函数，在状态机中调用进行状态切换
    {
        var newState = state switch
        {
            NPCState.Patrol => patrolState,
            NPCState.Chase => chaseState,
            _ => null
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
        rb.velocity = Vector2.zero;    //受击时先把速度降为零
        Vector2 dir = new Vector2(transform.position.x - attacker.position.x, 0).normalized;
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
    }

    public void EnemyDead()
    {
        if (isDead) return;

        gameObject.layer = 2;
        isDead = true;
        animator.SetBool("isDead", true);
        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.UpdateTaskProgress(enemyID, 1);
        }
        StartCoroutine(DissolveAndDestroy());
    }

    private IEnumerator DissolveAndDestroy()
    {
        yield return new WaitForSeconds(0.5f);

        float counter = 0f;
        float duration = 1f; //溶解持续1秒

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float val = Mathf.Lerp(0f, 1.1f, counter / duration); //确保完全消散

            //设置Shader属性
            spriteRenderer.GetPropertyBlock(propBlock);
            propBlock.SetFloat(dissolveID, val);
            spriteRenderer.SetPropertyBlock(propBlock);

            yield return null;
        }

        //销毁物体
        Destroy(gameObject);
    }


    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + (Vector3)centerOffset, checkSize);   //红色方框表示检测起始点
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + (Vector3)centerOffset + new Vector3(-checkDistance, 0, 0), checkSize);  //蓝色方框表示检测玩家范围
    }
}
